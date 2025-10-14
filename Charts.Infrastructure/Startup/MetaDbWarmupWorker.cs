// ============================================================================
// MetaDbWarmupWorker.cs
// ============================================================================
using Charts.Api.Domain.Interfaces;
using Charts.Api.Infrastructure.Databases;
using Charts.Api.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Charts.Api.Infrastructure.Startup;

public sealed class MetaDbWarmupWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<MetaDbWarmupWorker> _logger;
    private readonly IHostEnvironment _env;
    private readonly MetaDbOptions _options;
    private readonly IAppReadiness _readiness;

    public MetaDbWarmupWorker(
        IServiceProvider services,
        ILogger<MetaDbWarmupWorker> logger,
        IHostEnvironment env,
        IOptions<MetaDbOptions> options,
        IAppReadiness readiness)
    {
        _services = services;
        _logger = logger;
        _env = env;
        _options = options.Value;
        _readiness = readiness;
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
                    _logger.LogWarning("Retry {Attempt}/{Max} in {Delay}s", attempt, _options.RetryAttempts, delay.TotalSeconds);
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
                throw;
            }

            _logger.LogWarning("Continuing without MetaDb");
        }
    }

    private async Task InitializeAsync(AppDbContext db, CancellationToken ct)
    {
        // 1. Проверка подключения
        _logger.LogInformation("Step 1/3: Checking connection...");
        if (!await db.Database.CanConnectAsync(ct))
        {
            throw new Exception("Cannot connect to MetaDb");
        }
        _logger.LogInformation("✓ Connected");

        // 2. Пересоздание БД (только Dev!)
        if (_options.RecreateOnStartup)
        {
            if (!_env.IsDevelopment())
            {
                throw new Exception("RecreateOnStartup is only allowed in Development!");
            }

            _logger.LogWarning("Step 2/3: ⚠️ Recreating database...");
            await db.Database.EnsureDeletedAsync(ct);
            _logger.LogInformation("✓ Database deleted");
        }
        else
        {
            _logger.LogInformation("Step 2/3: Skipped (RecreateOnStartup=false)");
        }

        // 3. Применение миграций
        _logger.LogInformation("Step 3/3: Applying migrations...");

        if (_options.AutoMigrate)
        {
            try
            {
                // Проверяем pending миграции
                var pending = await db.Database.GetPendingMigrationsAsync(ct);
                var applied = await db.Database.GetAppliedMigrationsAsync(ct);

                _logger.LogInformation("Applied: {Applied}, Pending: {Pending}",
                    applied.Count(), pending.Count());

                if (pending.Any() || !applied.Any())
                {
                    _logger.LogInformation("Applying {Count} migration(s)...", pending.Count());
                    await db.Database.MigrateAsync(ct);
                    _logger.LogInformation("✓ Migrations applied");
                }
                else
                {
                    _logger.LogInformation("✓ Database up to date");
                }
            }
            catch (Exception ex)
            {
                // Если ошибка при чтении истории - значит БД пустая, просто мигрируем
                _logger.LogWarning(ex, "Error checking migrations, will apply all migrations");
                await db.Database.MigrateAsync(ct);
                _logger.LogInformation("✓ Migrations applied");
            }
        }
        else
        {
            // Проверяем что таблицы существуют
            _logger.LogInformation("AutoMigrate=false, checking schema...");
            var canQuery = await CanQueryDatabaseAsync(db, ct);

            if (!canQuery)
            {
                throw new Exception("Database schema missing. Run: dotnet ef database update");
            }

            _logger.LogInformation("✓ Schema exists");
        }
    }

    private async Task<bool> CanQueryDatabaseAsync(AppDbContext db, CancellationToken ct)
    {
        try
        {
            await db.Database.ExecuteSqlRawAsync(
                "SELECT 1 FROM information_schema.tables WHERE table_name = 'databases' LIMIT 1", ct);
            return true;
        }
        catch
        {
            return false;
        }
    }
}