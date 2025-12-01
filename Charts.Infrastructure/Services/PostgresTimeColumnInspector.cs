using System.Data.Common;
using Npgsql;

namespace Charts.Infrastructure.Services;

public sealed class PostgresTimeColumnInspector : ITimeColumnInspector
{
    public async Task<TimeColumnKind> GetKindAsync(DbConnection con, string entity, string timeField, CancellationToken ct)
    {
        if (con is not NpgsqlConnection npg)
            throw new NotSupportedException("TimeColumnInspector: only Postgres (Npgsql) supported");

        var (schema, table) = ParseEntityName(entity);
        string sql =
            schema is null
            ? """
                   select data_type
                   from information_schema.columns
                   where table_schema = current_schema()
                     and table_name = $1
                     and column_name = $2
                   limit 1
                   """
            : """
                   select data_type
                   from information_schema.columns
                   where table_schema = $1
                     and table_name = $2
                     and column_name = $3
                   limit 1
                   """;
        await using var cmd = new NpgsqlCommand(sql, npg);
        if (schema is null)
        {
            cmd.Parameters.AddWithValue(table);
            cmd.Parameters.AddWithValue(timeField);
        }
        else
        {
            cmd.Parameters.AddWithValue(schema);
            cmd.Parameters.AddWithValue(table);
            cmd.Parameters.AddWithValue(timeField);
        }
        var type = await cmd.ExecuteScalarAsync(ct) as string ?? string.Empty;
        return type.Equals("timestamp with time zone", StringComparison.OrdinalIgnoreCase)
            ? TimeColumnKind.Timestamptz
            : TimeColumnKind.Timestamp;
    }

    private static (string? schema, string table) ParseEntityName(string entity)
    {
        var i = entity.IndexOf('.');
        if (i < 0) return (null, entity.Trim());
        return (entity[..i].Trim(), entity[(i + 1)..].Trim());
    }
}
