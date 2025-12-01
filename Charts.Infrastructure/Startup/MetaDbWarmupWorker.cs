// ============================================================================
// MetaDbWarmupWorker.cs
// ============================================================================

using Charts.Domain.Interfaces;
using Charts.Infrastructure.Databases;
using Charts.Infrastructure.Databases.Seeder;
using Charts.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Charts.Infrastructure.Startup;

/// <summary>
/// Фоновый worker для инициализации мета-БД (создание, миграции, проверка подключения).
/// Работает с retry логикой и устанавливает флаг готовности приложения.
/// </summary>
public sealed class MetaDbWarmupWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<MetaDbWarmupWorker> _logger;
    private readonly IHostEnvironment _env;
    private readonly MetaDbOptions _options;
    private readonly IAppReadiness _readiness;
    private readonly IConfiguration _configuration;  // ← ДОБАВИТЬ

    public MetaDbWarmupWorker(
        IServiceProvider services,
        ILogger<MetaDbWarmupWorker> logger,
        IHostEnvironment env,
        IOptions<MetaDbOptions> options,
        IAppReadiness readiness,
        IConfiguration configuration)  // ← ДОБАВИТЬ
    {
        _services = services;
        _logger = logger;
        _env = env;
        _options = options.Value;
        _readiness = readiness;
        _configuration = configuration;  // ← ДОБАВИТЬ
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("MetaDb initialization started (Environment: {Env})", _env.EnvironmentName);

        try
        {
            // Retry с exponential backoff
            for (int attempt = 1; attempt <= _options.RetryAttempts; attempt++)
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    await InitializeAsync(db, ct);

                    _readiness.SetReady();
                    _logger.LogInformation("✓ MetaDb ready");
                    return;
                }
                catch when (attempt < _options.RetryAttempts)
                {
                    var delay = TimeSpan.FromSeconds(_options.RetryDelaySeconds * Math.Pow(2, attempt - 1));
                    _logger.LogWarning("Retry {Attempt}/{Max} in {Delay}s",
                        attempt, _options.RetryAttempts, delay.TotalSeconds);
                    await Task.Delay(delay, ct);
                }
            }

            throw new Exception("MetaDb initialization failed after all retries");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MetaDb initialization failed");

            if (_options.FailOnConnectionError)
            {
                _logger.LogCritical("Terminating application due to FailOnConnectionError=true");
                throw;
            }

            _logger.LogWarning("Continuing without MetaDb (FailOnConnectionError=false)");
            _readiness.SetReady(); // разрешаем запуск без БД
        }
    }

    private async Task InitializeAsync(AppDbContext db, CancellationToken ct)
    {
        // 1. Создаём БД если её нет
        _logger.LogInformation("Step 1/4: Ensuring database exists...");
        await EnsureDatabaseExistsAsync(db, ct);
        _logger.LogInformation("✓ Database exists");

        // 2. Проверка подключения
        _logger.LogInformation("Step 2/4: Checking connection...");
        if (!await db.Database.CanConnectAsync(ct))
        {
            throw new Exception("Cannot connect to MetaDb");
        }
        _logger.LogInformation("✓ Connected");

        // 3. Пересоздание БД (только Dev!)
        if (_options.RecreateOnStartup)
        {
            if (!_env.IsDevelopment())
            {
                throw new InvalidOperationException(
                    "RecreateOnStartup=true is only allowed in Development environment!");
            }

            _logger.LogWarning("Step 3/4: ⚠️ Recreating database (RecreateOnStartup=true)...");
            await db.Database.EnsureDeletedAsync(ct);
            _logger.LogInformation("✓ Database deleted");

            await EnsureDatabaseExistsAsync(db, ct);
            _logger.LogInformation("✓ Database recreated");
        }
        else
        {
            _logger.LogInformation("Step 3/4: Skipped (RecreateOnStartup=false)");
        }

        // 4. Применение миграций
        _logger.LogInformation("Step 4/4: Applying migrations...");

        if (_options.AutoMigrate)
        {
            await ApplyMigrationsAsync(db, ct);
        }
        else
        {
            await VerifySchemaExistsAsync(db, ct);
        }

        if (_options.RunSeeders)
        {
            await RunSeedersAsync(ct);
        }

        // 5. Инициализация реестра баз данных
        _logger.LogInformation("Step 6/6: Initializing Database Registry...");
        using var scope = _services.CreateScope();
        var registry = scope.ServiceProvider.GetRequiredService<IDatabaseRegistry>();
        await registry.InitializeAsync(ct);
        _logger.LogInformation("✓ Database Registry initialized with {Count} databases",
            registry.Snapshot.Count);
    }

    /// <summary>
    /// Запускает все зарегистрированные сидеры через SeederPipeline.
    /// </summary>
    private async Task RunSeedersAsync(CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("Step 5/5: Running seeders...");

            using var scope = _services.CreateScope();

            // Получаем SeederPipeline (если он зарегистрирован)
            var pipeline = scope.ServiceProvider.GetService<SeederPipeline>();

            if (pipeline == null)
            {
                _logger.LogWarning("SeederPipeline not registered, skipping seeders");
                return;
            }

            await pipeline.ExecuteAsync(_services);

            _logger.LogInformation("✓ All seeders completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to run seeders");

            if (_options.FailOnSeederError)
            {
                throw;
            }

            _logger.LogWarning("Continuing without seeders");
        }
    }

    /// <summary>
    /// Создаёт PostgreSQL БД если её нет.
    /// </summary>
    private async Task EnsureDatabaseExistsAsync(AppDbContext db, CancellationToken ct)
    {
        // ВАЖНО: Получаем connection string из конфигурации, а НЕ из DbContext!
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("ConnectionStrings:DefaultConnection is missing");

        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var targetDbName = builder.Database;

        if (string.IsNullOrEmpty(targetDbName))
        {
            _logger.LogWarning("Database name not specified in connection string, skipping check");
            return;
        }

        // Создаем connection string для служебной БД "postgres"
        builder.Database = "postgres";
        var masterConnectionString = builder.ToString();

        _logger.LogDebug("Master connection string (without password): {ConnectionString}",
            MaskPassword(masterConnectionString));

        await using var masterConn = new NpgsqlConnection(masterConnectionString);

        try
        {
            _logger.LogDebug("Connecting to 'postgres' database to check if '{DbName}' exists...", targetDbName);
            await masterConn.OpenAsync(ct);

            // Проверяем существование целевой БД
            await using var checkCmd = new NpgsqlCommand(
                "SELECT 1 FROM pg_database WHERE datname = $1", masterConn);
            checkCmd.Parameters.AddWithValue(targetDbName);

            var exists = await checkCmd.ExecuteScalarAsync(ct);

            if (exists == null)
            {
                _logger.LogWarning("Database '{DbName}' does not exist. Creating...", targetDbName);

                // Создаём БД с минимальными параметрами
                var safeName = targetDbName.Replace("\"", "\"\"");
                var createSql = $"CREATE DATABASE \"{safeName}\"";

                await using var createCmd = new NpgsqlCommand(createSql, masterConn);
                await createCmd.ExecuteNonQueryAsync(ct);

                _logger.LogInformation("✓ Database '{DbName}' created successfully", targetDbName);
            }
            else
            {
                _logger.LogDebug("Database '{DbName}' already exists", targetDbName);
            }
        }
        catch (PostgresException pgEx) when (pgEx.SqlState == "42P04")
        {
            _logger.LogDebug("Database '{DbName}' already exists (duplicate error)", targetDbName);
        }
        catch (NpgsqlException npgEx)
        {
            _logger.LogError(npgEx, "Npgsql error while ensuring database '{DbName}' exists", targetDbName);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while ensuring database '{DbName}' exists", targetDbName);
            throw;
        }
    }

    /// <summary>
    /// Маскирует пароль в connection string для логирования.
    /// </summary>
    private static string MaskPassword(string connectionString)
    {
        try
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                Password = "***"
            };
            return builder.ToString();
        }
        catch
        {
            return "[unable to parse]";
        }
    }

    /// <summary>
    /// Применяет все pending миграции.
    /// </summary>
    private async Task ApplyMigrationsAsync(AppDbContext db, CancellationToken ct)
    {
        try
        {
            // Проверяем pending и applied миграции
            var pending = await db.Database.GetPendingMigrationsAsync(ct);
            var applied = await db.Database.GetAppliedMigrationsAsync(ct);

            _logger.LogInformation("Migrations: Applied={Applied}, Pending={Pending}",
                applied.Count(), pending.Count());

            if (pending.Any())
            {
                _logger.LogInformation("Applying {Count} migration(s): {Names}",
                    pending.Count(), string.Join(", ", pending));

                await db.Database.MigrateAsync(ct);

                _logger.LogInformation("✓ Migrations applied successfully");
            }
            else if (!applied.Any())
            {
                // БД пустая, но нет pending миграций - странная ситуация
                _logger.LogWarning("No migrations found. Creating schema from model...");
                await db.Database.MigrateAsync(ct);
            }
            else
            {
                _logger.LogInformation("✓ Database schema is up to date");
            }
        }
        catch (Exception ex)
        {
            // Если не можем прочитать историю миграций - значит БД пустая
            _logger.LogWarning(ex, "Error checking migration history. Applying all migrations...");

            try
            {
                await db.Database.MigrateAsync(ct);
                _logger.LogInformation("✓ Migrations applied successfully");
            }
            catch (Exception migrateEx)
            {
                _logger.LogError(migrateEx, "Failed to apply migrations");
                throw;
            }
        }
    }

    /// <summary>
    /// Проверяет что схема БД существует (когда AutoMigrate=false).
    /// </summary>
    private async Task VerifySchemaExistsAsync(AppDbContext db, CancellationToken ct)
    {
        _logger.LogInformation("AutoMigrate=false, verifying schema exists...");

        var canQuery = await CanQueryDatabaseAsync(db, ct);

        if (!canQuery)
        {
            var msg = "Database schema is missing. " +
                     "Run: dotnet ef database update --project <YourInfrastructureProject>";
            _logger.LogError(msg);
            throw new InvalidOperationException(msg);
        }

        _logger.LogInformation("✓ Schema exists");
    }

    /// <summary>
    /// Проверяет что можно выполнить запрос к основной таблице.
    /// </summary>
    private async Task<bool> CanQueryDatabaseAsync(AppDbContext db, CancellationToken ct)
    {
        try
        {
            // Проверяем существование таблицы 'databases' (или любой вашей основной таблицы)
            var result = await db.Database.ExecuteSqlRawAsync(
                "SELECT 1 FROM information_schema.tables " +
                "WHERE table_schema = 'public' AND table_name = 'databases' LIMIT 1",
                ct);

            return result >= 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Экранирует SQL идентификатор (защита от SQL injection).
    /// </summary>
    private static string QuoteIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Identifier cannot be empty", nameof(identifier));

        // Удаляем опасные символы и оборачиваем в кавычки
        var cleaned = identifier.Replace("\"", "\"\"");
        return $"\"{cleaned}\"";
    }
}