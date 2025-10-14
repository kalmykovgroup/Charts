using AutoMapper;
using Charts.Api.Application.Interfaces;
using Charts.Api.Application.Contracts;
using Charts.Api.Application.Contracts.Metadata.Dtos;
using Charts.Api.Application.Interfaces.Repositories;
using Charts.Api.Application.QueryAndCommands.Metadata.Databases;
using Charts.Api.Domain.Contracts.Types;
using Charts.Api.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Charts.Api.Application.Handlers.Metadata.Databases;
 

public sealed class GetAllDatabasesHandler(
    IDatabaseRepository repo,
    IMapper mapper,
    IDbProviderRegistry providers,
    IEntityMetadataService metadata,
    ILogger<GetAllDatabasesHandler> log
) : IRequestHandler<GetAllDatabasesQuery, ApiResponse<List<DatabaseDto>>>
{
    public async Task<ApiResponse<List<DatabaseDto>>> Handle(GetAllDatabasesQuery request, CancellationToken ct)
    {
        try
        {
            var items = await repo.GetAllAsync(ct);
            var databases = mapper.Map<List<DatabaseDto>>(items);

            foreach (var db in databases)
            {
                try
                {
                    await using var con = await TryOpenAsync(db, providers, ct);

                    db.DatabaseVersion = await GetVersionAsync(con, db.Provider, ct);

                    var entities = await metadata.GetEntitiesAsync(con, db.Provider, ct);
                    db.Entities = entities.ToList();

                    db.Availability = DatabaseAvailability.Online;
                    db.LastConnectivityAt = DateTime.UtcNow;
                    db.LastConnectivityError = null;
                }
                catch (Exception ex)
                {
                    log.LogWarning(ex, "Не удалось подключиться к БД {Name}", db.Name);

                    db.Availability = DatabaseAvailability.Offline;
                    db.LastConnectivityAt = DateTime.UtcNow;
                    db.LastConnectivityError = ex.Message;

                    db.Entities = new List<EntityDto>(); // чтобы UI не ждал полей
                }
            }

            return ApiResponse<List<DatabaseDto>>.Ok(databases);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<DatabaseDto>>.Fail(ex.Message, ex);
        }
    }

    // ---------- helpers ----------

    private static async Task<DbConnection> TryOpenAsync(
        DatabaseDto db,
        IDbProviderRegistry providers,
        CancellationToken ct)
    {
        // сопоставляем enum провайдера с ключом фабрики
        var providerKey = db.Provider switch
        {
            DbProviderType.PostgreSql => "Npgsql",              // зарегай такой ключ в DbProviderRegistry
            // DbProviderType.SqlServer => "Microsoft.Data.SqlClient",
            // DbProviderType.MySql    => "MySqlConnector",
            _ => throw new NotSupportedException($"Провайдер {db.Provider} не поддержан")
        };

        var factory = providers.GetFactory(providerKey);
        var con = factory.CreateConnection()
                  ?? throw new InvalidOperationException($"Factory.CreateConnection() вернул null для {providerKey}");

        con.ConnectionString = db.ConnectionString;
        await con.OpenAsync(ct);
        return con;
    }

    private static async Task<string> GetVersionAsync(
        DbConnection con,
        DbProviderType provider,
        CancellationToken ct)
    {
        await using var cmd = con.CreateCommand();

        switch (provider)
        {
            case DbProviderType.PostgreSql:
                // краткая версия сервера, напр. "16.4"
                cmd.CommandText = "show server_version";
                var v = await cmd.ExecuteScalarAsync(ct);
                return Convert.ToString(v) ?? string.Empty;

            // case DbProviderType.SqlServer:
            //     cmd.CommandText = "SELECT CONVERT(varchar(128), SERVERPROPERTY('ProductVersion'))";
            //     return Convert.ToString(await cmd.ExecuteScalarAsync(ct)) ?? string.Empty;

            default:
                // универсальный fallback
                cmd.CommandText = "select version()";
                var any = await cmd.ExecuteScalarAsync(ct);
                return Convert.ToString(any) ?? string.Empty;
        }
    }
}
