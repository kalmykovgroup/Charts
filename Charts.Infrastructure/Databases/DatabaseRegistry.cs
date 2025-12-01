using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Types;
using Charts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Charts.Infrastructure.Databases;

public sealed class DatabaseRegistry : IDatabaseRegistry
{
    private readonly ILogger<DatabaseRegistry> _log;
    private readonly IServiceProvider _sp;
    private readonly object _lock = new();

    private Dictionary<Guid, RegisteredDatabase> _byId = new();
    private Dictionary<string, Guid> _nameToId = new(StringComparer.OrdinalIgnoreCase);

    public DatabaseRegistry(ILogger<DatabaseRegistry> log, IServiceProvider sp)
    {
        _log = log;
        _sp = sp;
    }

    public IReadOnlyDictionary<Guid, RegisteredDatabase> Snapshot => _byId;

    public async Task InitializeAsync(CancellationToken ct = default) => await ReloadAsync(ct);

    public async Task ReloadAsync(CancellationToken ct = default)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var rows = await db.Databases
            .Where(d => d.Status == EntityStatus.Active && !d.IsDeleted)
            .Select(d => new
            {
                d.Id,
                d.Name,
                d.DatabaseVersion,
                d.ConnectionString,
                d.Provider
            })
            .ToListAsync(ct);

        var map = new Dictionary<Guid, RegisteredDatabase>(rows.Count);
        var nameIx = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

        foreach (var d in rows)
        {
            var registered = CreateRegisteredDatabase(d.Id, d.Name, d.DatabaseVersion, d.ConnectionString, d.Provider);
            if (registered != null)
            {
                map[d.Id] = registered;
                nameIx[d.Name] = d.Id;
            }
        }

        lock (_lock)
        {
            _byId = map;
            _nameToId = nameIx;
        }

        _log.LogInformation("Database registry reloaded. Count={Count}", _byId.Count);
    }

    public async Task RegisterAsync(Guid id, CancellationToken ct = default)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var entity = await db.Databases
            .Where(d => d.Id == id && d.Status == EntityStatus.Active && !d.IsDeleted)
            .Select(d => new
            {
                d.Id,
                d.Name,
                d.DatabaseVersion,
                d.ConnectionString,
                d.Provider
            })
            .FirstOrDefaultAsync(ct);

        if (entity == null)
        {
            _log.LogWarning("Cannot register database {Id}: not found or not active", id);
            return;
        }

        var registered = CreateRegisteredDatabase(entity.Id, entity.Name, entity.DatabaseVersion, entity.ConnectionString, entity.Provider);
        if (registered != null)
        {
            lock (_lock)
            {
                _byId[entity.Id] = registered;
                _nameToId[entity.Name] = entity.Id;
            }
            _log.LogInformation("Database registered: {Name} ({Id})", entity.Name, entity.Id);
        }
    }

    public Task UnregisterAsync(Guid id, CancellationToken ct = default)
    {
        lock (_lock)
        {
            if (_byId.TryGetValue(id, out var db))
            {
                _byId.Remove(id);
                _nameToId.Remove(db.Key);
                _log.LogInformation("Database unregistered: {Name} ({Id})", db.Key, id);
            }
        }
        return Task.CompletedTask;
    }

    public RegisteredDatabase ResolveById(Guid id)
    {
        lock (_lock)
        {
            if (_byId.TryGetValue(id, out var db))
                return db;
        }
        throw new KeyNotFoundException($"Database with id '{id}' not found");
    }

    public RegisteredDatabase ResolveByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        lock (_lock)
        {
            if (_nameToId.TryGetValue(name, out var id) && _byId.TryGetValue(id, out var db))
                return db;
        }
        throw new KeyNotFoundException($"Database '{name}' not found");
    }

    public async Task<RegisteredDatabase> ResolveByIdAsync(Guid id, CancellationToken ct = default)
    {
        // Сначала проверяем кеш
        lock (_lock)
        {
            if (_byId.TryGetValue(id, out var cached))
                return cached;
        }

        // Если нет в кеше — пробуем загрузить из БД
        _log.LogDebug("Database {Id} not in cache, loading from DB...", id);
        await RegisterAsync(id, ct);

        lock (_lock)
        {
            if (_byId.TryGetValue(id, out var db))
                return db;
        }

        throw new KeyNotFoundException($"Database with id '{id}' not found");
    }

    public async Task<RegisteredDatabase> ResolveByNameAsync(string name, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        // Сначала проверяем кеш
        lock (_lock)
        {
            if (_nameToId.TryGetValue(name, out var id) && _byId.TryGetValue(id, out var cached))
                return cached;
        }

        // Если нет в кеше — пробуем загрузить из БД
        _log.LogDebug("Database '{Name}' not in cache, loading from DB...", name);

        using var scope = _sp.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var entity = await dbContext.Databases
            .Where(d => d.Name == name && d.Status == EntityStatus.Active && !d.IsDeleted)
            .Select(d => new { d.Id })
            .FirstOrDefaultAsync(ct);

        if (entity != null)
        {
            await RegisterAsync(entity.Id, ct);

            lock (_lock)
            {
                if (_byId.TryGetValue(entity.Id, out var db))
                    return db;
            }
        }

        throw new KeyNotFoundException($"Database '{name}' not found");
    }

    public bool TryResolveById(Guid id, out RegisteredDatabase db)
    {
        lock (_lock)
        {
            return _byId.TryGetValue(id, out db!);
        }
    }

    public bool TryResolveByName(string name, out RegisteredDatabase db)
    {
        db = default!;
        lock (_lock)
        {
            return _nameToId.TryGetValue(name, out var id) && _byId.TryGetValue(id, out db);
        }
    }

    public async Task<ConnectionTestResult> TestConnectionAsync(string connectionString, DbProviderType provider, CancellationToken ct = default)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            switch (provider)
            {
                case DbProviderType.PostgreSql:
                {
                    var csb = new NpgsqlConnectionStringBuilder(connectionString);
                    if (string.Equals(csb.Host, "localhost", StringComparison.OrdinalIgnoreCase))
                        csb.Host = "127.0.0.1";
                    csb.Encoding = "UTF8";
                    csb.ClientEncoding = "UTF8";
                    csb.Options = "-c lc_messages=en_US.UTF-8";
                    csb.Timeout = 10;

                    var dsb = new NpgsqlDataSourceBuilder(csb.ConnectionString);
                    await using var ds = dsb.Build();
                    await using var connection = await ds.OpenConnectionAsync(ct);

                    sw.Stop();
                    _log.LogInformation("Connection test successful. Server: {Version}, Time: {Time}ms", connection.ServerVersion, sw.ElapsedMilliseconds);

                    return new ConnectionTestResult(true, connection.ServerVersion, null, sw.ElapsedMilliseconds);
                }

                case DbProviderType.MySql:
                    return new ConnectionTestResult(false, null, "MySql provider not implemented yet", sw.ElapsedMilliseconds);

                case DbProviderType.SqlServer:
                    return new ConnectionTestResult(false, null, "SqlServer provider not implemented yet", sw.ElapsedMilliseconds);

                default:
                    return new ConnectionTestResult(false, null, $"Unknown provider: {provider}", sw.ElapsedMilliseconds);
            }
        }
        catch (Exception ex)
        {
            sw.Stop();
            _log.LogWarning(ex, "Connection test failed");
            return new ConnectionTestResult(false, null, Helpers.PostgresErrorHelper.GetMessage(ex), sw.ElapsedMilliseconds);
        }
    }

    private RegisteredDatabase? CreateRegisteredDatabase(Guid id, string name, string? version, string connectionString, DbProviderType provider)
    {
        try
        {
            switch (provider)
            {
                case DbProviderType.PostgreSql:
                {
                    var csb = new NpgsqlConnectionStringBuilder(connectionString);
                    if (string.Equals(csb.Host, "localhost", StringComparison.OrdinalIgnoreCase))
                        csb.Host = "127.0.0.1";
                    csb.Encoding = "UTF8";
                    csb.ClientEncoding = "UTF8";
                    csb.Options = "-c lc_messages=en_US.UTF-8";

                    var dsb = new NpgsqlDataSourceBuilder(csb.ConnectionString);
                    var ds = dsb.Build();

                    return new RegisteredDatabase
                    {
                        Id = id,
                        Key = name,
                        DatabaseVersion = version ?? "",
                        ConnectionString = csb.ConnectionString,
                        Provider = DbProviderType.PostgreSql,
                        RawDataSource = ds,
                        OpenConnectionAsync = async ct2 => await ds.OpenConnectionAsync(ct2)
                    };
                }

                case DbProviderType.MySql:
                    _log.LogWarning("MySql provider not implemented for database {Name}", name);
                    return null;

                case DbProviderType.SqlServer:
                    _log.LogWarning("SqlServer provider not implemented for database {Name}", name);
                    return null;

                default:
                    _log.LogWarning("Unknown provider {Provider} for database {Name}", provider, name);
                    return null;
            }
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to create registered database for {Name}", name);
            return null;
        }
    }
}
