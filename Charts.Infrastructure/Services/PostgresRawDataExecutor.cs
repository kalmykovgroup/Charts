using System.Data.Common;
using System.Globalization;
using Charts.Domain.Contracts.Charts.Dtos;
using Npgsql;

namespace Charts.Infrastructure.Services;

public sealed class PostgresRawDataExecutor : IRawDataExecutor
{
    public async Task<List<RawPointDto>> ExecutePointsAsync(DbConnection con, SqlRequest request, CancellationToken ct)
    {
        if (con is not NpgsqlConnection npg)
            throw new NotSupportedException("RawDataExecutor: only Postgres (Npgsql) supported");

        var result = new List<RawPointDto>(4096);
        await using var cmd = new NpgsqlCommand(request.Sql, npg);
        if (request.Parameters?.Count > 0)
            cmd.Parameters.AddRange(request.Parameters.ToArray());

        await using var rdr = await cmd.ExecuteReaderAsync(ct);
        while (await rdr.ReadAsync(ct))
        {
            DateTimeOffset t = rdr.GetFieldValue<DateTimeOffset>(0);
            double? v = rdr.IsDBNull(1) ? null : Convert.ToDouble(rdr.GetValue(1), CultureInfo.InvariantCulture);
            result.Add(new RawPointDto(t.ToUnixTimeMilliseconds(), v));
        }
        return result;
    }

    public async Task<long?> ExecuteEdgeTimeAsync(DbConnection con, SqlRequest request, CancellationToken ct)
    {
        if (con is not NpgsqlConnection npg)
            throw new NotSupportedException("EdgeTime: only Postgres (Npgsql) supported");

        await using var cmd = new NpgsqlCommand(request.Sql, npg);
        if (request.Parameters?.Count > 0)
            cmd.Parameters.AddRange(request.Parameters.ToArray());

        var scalar = await cmd.ExecuteScalarAsync(ct);
        long? t = scalar is DateTimeOffset dtf ? dtf.ToUnixTimeMilliseconds() : scalar is DateTime dt ? new DateTimeOffset(dt).ToUnixTimeMilliseconds() : null;
        return t;
    }
}