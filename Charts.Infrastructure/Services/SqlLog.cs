using Npgsql;
using NpgsqlTypes;
using System.Globalization;
using System.Text;

namespace Charts.Api.Infrastructure.Services;


 

internal static class SqlLog
{
    // Короткое описание параметра (имя, тип, значение, CLR-тип)
    public static string DescribeParam(NpgsqlParameter p)
    {
        var type = p.NpgsqlDbType.ToString();
        var name = p.ParameterName;
        var value = p.Value is null or DBNull ? "NULL" : ToPreview(p.Value);
        var clr = p.Value?.GetType().Name ?? "null";
        return $"{name}::{type} = {value} ({clr})";
    }

    public static string DescribeParams(IEnumerable<NpgsqlParameter> ps)
        => string.Join("; ", ps.Select(DescribeParam));

    // Аккуратно «раскрываем» SQL для отладки (только в Debug-логах!)
    public static string ExpandSqlForLog(string sql, IEnumerable<NpgsqlParameter> ps)
    {
        // чтобы не повредить позиции, заменяем имена по убыванию длины
        var ordered = ps.OrderByDescending(x => x.ParameterName.Length).ToArray();
        foreach (var p in ordered)
        {
            sql = sql.Replace("@" + p.ParameterName, ToSqlLiteral(p), StringComparison.Ordinal);
        }
        return sql;
    }

    private static string ToSqlLiteral(NpgsqlParameter p)
    {
        if (p.Value is null or DBNull) return "NULL";

        var t = p.NpgsqlDbType;
        var v = p.Value;

        if ((t & NpgsqlDbType.Array) == NpgsqlDbType.Array)
        {
            var elemType = t & ~NpgsqlDbType.Array;
            var sb = new StringBuilder("{");
            if (v is System.Collections.IEnumerable seq)
            {
                bool first = true;
                foreach (var it in seq)
                {
                    if (!first) sb.Append(',');
                    sb.Append(ToArrayItemLiteral(elemType, it));
                    first = false;
                }
            }
            sb.Append('}');
            return $"ARRAY{sb}";
        }

        return t switch
        {
            NpgsqlDbType.Boolean => (bool)v! ? "TRUE" : "FALSE",
            NpgsqlDbType.Integer or NpgsqlDbType.Bigint or NpgsqlDbType.Smallint
                or NpgsqlDbType.Double or NpgsqlDbType.Real or NpgsqlDbType.Numeric
                => Convert.ToString(v, CultureInfo.InvariantCulture)!,

            NpgsqlDbType.TimestampTz => v switch
            {
                DateTimeOffset dto => $"timestamptz '{dto:yyyy-MM-dd HH:mm:ss.ffffffK}'",
                DateTime dt => $"timestamptz '{ToUtc(dt):yyyy-MM-dd HH:mm:ss.ffffffK}'",
                _ => $"'{v}'"
            },
            NpgsqlDbType.Timestamp => v switch
            {
                DateTimeOffset dto => $"timestamp '{dto.UtcDateTime:yyyy-MM-dd HH:mm:ss.ffffff}'",
                DateTime dt => $"timestamp '{ToUnspec(dt):yyyy-MM-dd HH:mm:ss.ffffff}'",
                _ => $"'{v}'"
            },
            NpgsqlDbType.Date => $"date '{((DateTime)v!).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}'",

            NpgsqlDbType.Uuid => $"'{v}'::uuid",
            NpgsqlDbType.Jsonb => $"'{Escape(v.ToString())}'::jsonb",
            NpgsqlDbType.Json => $"'{Escape(v.ToString())}'::json",
            _ => $"'{Escape(Convert.ToString(v, CultureInfo.InvariantCulture) ?? "")}'"
        };
    }

    private static string ToArrayItemLiteral(NpgsqlDbType elem, object? v)
    {
        if (v is null) return "NULL";
        return elem switch
        {
            NpgsqlDbType.Integer or NpgsqlDbType.Bigint or NpgsqlDbType.Smallint
                or NpgsqlDbType.Double or NpgsqlDbType.Real or NpgsqlDbType.Numeric
                => Convert.ToString(v, CultureInfo.InvariantCulture)!,
            _ => $"\"{Convert.ToString(v, CultureInfo.InvariantCulture)?.Replace("\"", "\\\"")}\""
        };
    }

    private static string ToPreview(object v)
    {
        if (v is Array arr) return $"[{arr.Length} items]";
        var s = Convert.ToString(v, CultureInfo.InvariantCulture) ?? "";
        return s.Length > 200 ? s.Substring(0, 200) + "…" : s;
    }

    private static string Escape(string s) => s.Replace("'", "''");

    private static DateTime ToUtc(DateTime dt)
    {
        if (dt.Kind == DateTimeKind.Utc) return dt;
        if (dt.Kind == DateTimeKind.Local) return dt.ToUniversalTime();
        return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
    }
    private static DateTime ToUnspec(DateTime dt)
        => dt.Kind == DateTimeKind.Unspecified ? dt : DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
}

