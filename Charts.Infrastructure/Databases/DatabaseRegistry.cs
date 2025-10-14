using Charts.Api.Application.Contracts;
using Charts.Api.Application.Interfaces;
using Charts.Api.Domain.Contracts.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data.Common;

namespace Charts.Api.Infrastructure.Databases;

public sealed class DatabaseRegistry : IDatabaseRegistry
{
    private readonly ILogger<DatabaseRegistry> _log;
    private readonly IServiceProvider _sp;   // создаём scope когда нужно

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
        // читаем «каталог баз» из твоей таблицы databases
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // подстрой под свою модель (важно: должен быть провайдер)
        var rows = await db.Databases
            .Where(d => d.DatabaseStatus == DatabaseStatus.Active && !d.IsDeleted)
            .Select(d => new
            {
                d.Id,
                d.Name,
                d.DatabaseVersion,
                d.ConnectionString,
                d.Provider // тип: DbProviderType (int) в БД
            })
            .ToListAsync(ct);

        var map = new Dictionary<Guid, RegisteredDatabase>(rows.Count);
        var nameIx = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

        foreach (var d in rows)
        {
            switch (d.Provider)
            {
                case DbProviderType.PostgreSql:
                    {
                        // Npgsql DataSource
                        var csb = new NpgsqlConnectionStringBuilder(d.ConnectionString);
                        if (string.Equals(csb.Host, "localhost", StringComparison.OrdinalIgnoreCase))
                            csb.Host = "127.0.0.1";

                        var dsb = new NpgsqlDataSourceBuilder(csb.ConnectionString);
                        // dsb.EnableDynamicJson(); // если нужно jsonb с динамическим сериализатором
                        var ds = dsb.Build();

                        map[d.Id] = new RegisteredDatabase
                        {
                            Id = d.Id,
                            Key = d.Name,
                            DatabaseVersion = d.DatabaseVersion ?? "",
                            ConnectionString = csb.ConnectionString,
                            Provider = DbProviderType.PostgreSql,
                            RawDataSource = ds,
                            OpenConnectionAsync = async ct2 => await ds.OpenConnectionAsync(ct2)
                        };
                        nameIx[d.Name] = d.Id;
                        break;
                    }

                case DbProviderType.MySql:
                    throw new NotSupportedException("Нужно реализовать для MySql");

                case DbProviderType.SqlServer:
                    throw new NotSupportedException("Нужно реализовать для SqlServer");

                default:
                    throw new NotSupportedException($"Неизвестный провайдер: {d.Provider}");
            }
        }

        _byId = map;
        _nameToId = nameIx;

        _log.LogInformation("Database registry reloaded. Count={Count}", _byId.Count);
    }

    public RegisteredDatabase ResolveById(Guid id) =>
        _byId.TryGetValue(id, out var db)
            ? db
            : throw new KeyNotFoundException($"Database with id '{id}' not found");

    public RegisteredDatabase ResolveByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        return _nameToId.TryGetValue(name, out var id) && _byId.TryGetValue(id, out var db)
            ? db
            : throw new KeyNotFoundException($"Database '{name}' not found");
    }

    public bool TryResolveById(Guid id, out RegisteredDatabase db) =>
        _byId.TryGetValue(id, out db!);

    public bool TryResolveByName(string name, out RegisteredDatabase db)
    {
        db = default!;
        return _nameToId.TryGetValue(name, out var id) && _byId.TryGetValue(id, out db);
    }
}
