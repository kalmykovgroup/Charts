using Charts.Api.Domain.Contracts.Template;
using Npgsql;
using NpgsqlTypes;
using System.Globalization;
using System.Text.Json;

namespace Charts.Api.Infrastructure.Services;

public static class PgParamFactory
{
    public static NpgsqlParameter FromReady(ReadySqlParam rp)
    {
        if (rp.Required && rp.Value is null)
            throw new ArgumentNullException(rp.Key, $"Param '{rp.Key}' is required.");

        var (dbType, isArray, elemType) = MapFieldType(rp.Field?.Type);
        var value = Unwrap(rp.Value);

        // КЛЮЧЕВОЕ ИСПРАВЛЕНИЕ: применяем коэрцию значения по типу
        if (isArray)
        {
            value = ToArray(value, elemType);
        }
        else
        {
            value = CoerceValue(value, elemType);
        }

        return new NpgsqlParameter(rp.Key, dbType)
        {
            Value = value ?? DBNull.Value
        };
    }

    // ---------- helpers ----------

    private static object? Unwrap(object? v)
    {
        if (v is not JsonElement je) return v;
        return je.ValueKind switch
        {
            JsonValueKind.String => je.GetString(),
            JsonValueKind.Number => je.TryGetInt64(out var li) ? li :
                                    je.TryGetDouble(out var d) ? d :
                                    je.GetRawText(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => je.GetRawText() // объект/массив как JSON-строка (если Field.Type=json/jsonb)
        };
    }

    // Возвращает полный dbtype (с признаком массива), флаг массива и базовый элементный тип
    private static (NpgsqlDbType dbType, bool isArray, NpgsqlDbType elem) MapFieldType(string? t)
    {
        if (string.IsNullOrWhiteSpace(t))
            return (NpgsqlDbType.Text, false, NpgsqlDbType.Text);

        t = t.Trim().ToLowerInvariant();
        bool isArray = t.EndsWith("[]");
        if (isArray) t = t[..^2];

        var elem = t switch
        {
            "text" or "varchar" or "citext" => NpgsqlDbType.Text,
            "bool" or "boolean" => NpgsqlDbType.Boolean,
            "uuid" => NpgsqlDbType.Uuid,
            "int" or "int4" or "integer" => NpgsqlDbType.Integer,
            "bigint" or "int8" => NpgsqlDbType.Bigint,
            "real" or "float4" => NpgsqlDbType.Real,
            "double" or "float8" => NpgsqlDbType.Double,
            "numeric" or "decimal" => NpgsqlDbType.Numeric,
            "date" => NpgsqlDbType.Date,
            "time" => NpgsqlDbType.Time,
            "timetz" => NpgsqlDbType.TimeTz,
            "timestamp" => NpgsqlDbType.Timestamp,
            "timestamptz" => NpgsqlDbType.TimestampTz,
            "jsonb" => NpgsqlDbType.Jsonb,
            "json" => NpgsqlDbType.Json,
            "bytea" => NpgsqlDbType.Bytea,
            "inet" => NpgsqlDbType.Inet,
            "cidr" => NpgsqlDbType.Cidr,
            "macaddr" => NpgsqlDbType.MacAddr,
            _ => NpgsqlDbType.Text
        };

        var dbType = isArray ? NpgsqlDbType.Array | elem : elem;
        return (dbType, isArray, elem);
    }

    /// <summary>
    /// НОВОЕ: коэрция одиночного значения по типу элемента
    /// </summary>
    private static object? CoerceValue(object? value, NpgsqlDbType elemType)
    {
        if (value is null or DBNull) return null;

        return elemType switch
        {
            NpgsqlDbType.Uuid => CoerceGuid(value),
            NpgsqlDbType.Integer => CoerceInt(value),
            NpgsqlDbType.Bigint => CoerceLong(value),
            NpgsqlDbType.Real or NpgsqlDbType.Double => CoerceDouble(value),
            NpgsqlDbType.Numeric => CoerceDecimal(value),
            NpgsqlDbType.Boolean => CoerceBool(value),
            NpgsqlDbType.Date => CoerceDateOnly(value),
            NpgsqlDbType.Timestamp => CoerceDateTime(value, false),
            NpgsqlDbType.TimestampTz => CoerceDateTime(value, true),
            _ => value // для text, json, bytea и т.д. оставляем как есть
        };
    }

    private static object? ToArray(object? value, NpgsqlDbType elemType)
    {
        if (value is null) return Array.Empty<object>();
        if (value is string s && elemType != NpgsqlDbType.Text)
        {
            // для не-текстовых массивов одиночная строка — это элемент
            return elemType switch
            {
                NpgsqlDbType.Uuid => new[] { CoerceGuid(s) },
                NpgsqlDbType.Integer => new[] { CoerceInt(s) },
                NpgsqlDbType.Bigint => new[] { CoerceLong(s) },
                NpgsqlDbType.Double => new[] { CoerceDouble(s) },
                _ => new[] { s }
            };
        }

        if (value is System.Collections.IEnumerable seq && value is not string)
        {
            switch (elemType)
            {
                case NpgsqlDbType.Text:
                    var strs = new List<string?>();
                    foreach (var it in seq) strs.Add(it?.ToString());
                    return strs.ToArray();

                case NpgsqlDbType.Integer:
                    var ints = new List<int>();
                    foreach (var it in seq)
                    {
                        var coerced = CoerceInt(it);
                        if (coerced is int i) ints.Add(i);
                    }
                    return ints.ToArray();

                case NpgsqlDbType.Bigint:
                    var longs = new List<long>();
                    foreach (var it in seq)
                    {
                        var coerced = CoerceLong(it);
                        if (coerced is long l) longs.Add(l);
                    }
                    return longs.ToArray();

                case NpgsqlDbType.Real:
                case NpgsqlDbType.Double:
                    var doubles = new List<double>();
                    foreach (var it in seq)
                    {
                        var coerced = CoerceDouble(it);
                        if (coerced is double d) doubles.Add(d);
                    }
                    return doubles.ToArray();

                case NpgsqlDbType.Uuid:
                    var guids = new List<Guid>();
                    foreach (var it in seq)
                    {
                        var coerced = CoerceGuid(it);
                        if (coerced is Guid g) guids.Add(g);
                    }
                    return guids.ToArray();

                case NpgsqlDbType.Date:
                    var dates = new List<DateTime>();
                    foreach (var it in seq)
                    {
                        var coerced = CoerceDateOnly(it);
                        if (coerced is DateTime dt) dates.Add(dt);
                    }
                    return dates.ToArray();

                case NpgsqlDbType.Timestamp:
                    var timestamps = new List<DateTime>();
                    foreach (var it in seq)
                    {
                        var coerced = CoerceDateTime(it, false);
                        if (coerced is DateTime dt) timestamps.Add(dt);
                    }
                    return timestamps.ToArray();

                case NpgsqlDbType.TimestampTz:
                    var timestamptzs = new List<DateTime>();
                    foreach (var it in seq)
                    {
                        var coerced = CoerceDateTime(it, true);
                        if (coerced is DateTime dt) timestamptzs.Add(dt);
                    }
                    return timestamptzs.ToArray();

                default:
                    // универсально, но менее типобезопасно — для редких типов
                    var objs = new List<object?>();
                    foreach (var it in seq) objs.Add(it);
                    return objs.ToArray();
            }
        }

        // одиночное значение → массив из одного элемента
        return elemType switch
        {
            NpgsqlDbType.Text => new[] { value?.ToString() },
            NpgsqlDbType.Uuid => new[] { CoerceGuid(value) },
            NpgsqlDbType.Integer => new[] { CoerceInt(value) },
            NpgsqlDbType.Bigint => new[] { CoerceLong(value) },
            NpgsqlDbType.Double => new[] { CoerceDouble(value) },
            _ => new object?[] { value }
        };
    }

    // ============== МЕТОДЫ КОЭРЦИИ ==============

    private static object? CoerceGuid(object? v)
    {
        if (v is null or DBNull) return null;
        if (v is Guid guid) return guid;
        if (v is string s && Guid.TryParse(s, out var g)) return g;
        throw new InvalidCastException($"Cannot convert '{v}' (type: {v.GetType().Name}) to Guid");
    }

    private static object? CoerceInt(object? v)
    {
        if (v is null or DBNull) return null;
        if (v is int i) return i;
        if (v is long l) return (int)l;
        if (v is string s && int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            return result;
        return Convert.ToInt32(v, CultureInfo.InvariantCulture);
    }

    private static object? CoerceLong(object? v)
    {
        if (v is null or DBNull) return null;
        if (v is long l) return l;
        if (v is int i) return (long)i;
        if (v is string s && long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            return result;
        return Convert.ToInt64(v, CultureInfo.InvariantCulture);
    }

    private static object? CoerceDouble(object? v)
    {
        if (v is null or DBNull) return null;
        if (v is double d) return d;
        if (v is float f) return (double)f;
        if (v is string s && double.TryParse(s.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
            return result;
        return Convert.ToDouble(v, CultureInfo.InvariantCulture);
    }

    private static object? CoerceDecimal(object? v)
    {
        if (v is null or DBNull) return null;
        if (v is decimal dec) return dec;
        if (v is string s && decimal.TryParse(s.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
            return result;
        return Convert.ToDecimal(v, CultureInfo.InvariantCulture);
    }

    private static object? CoerceBool(object? v)
    {
        if (v is null or DBNull) return null;
        if (v is bool b) return b;
        if (v is string s)
        {
            s = s.Trim().ToLowerInvariant();
            if (s is "true" or "1" or "yes" or "y" or "on") return true;
            if (s is "false" or "0" or "no" or "n" or "off") return false;
        }
        return Convert.ToBoolean(v, CultureInfo.InvariantCulture);
    }

    private static object? CoerceDateOnly(object? v)
    {
        if (v is null or DBNull) return null;
        if (v is DateTime dt) return dt.Date;
        if (v is DateTimeOffset dto) return dto.Date;
        if (v is string s && DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
            return d.Date;
        return Convert.ToDateTime(v, CultureInfo.InvariantCulture).Date;
    }

    private static object? CoerceDateTime(object? v, bool assumeUtc)
    {
        if (v is null or DBNull) return null;

        if (v is DateTime dt)
            return assumeUtc
                ? (dt.Kind == DateTimeKind.Utc ? dt
                 : dt.Kind == DateTimeKind.Local ? dt.ToUniversalTime()
                 : DateTime.SpecifyKind(dt, DateTimeKind.Utc))
                : DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);

        if (v is DateTimeOffset dto)
            return assumeUtc ? dto.UtcDateTime : dto.LocalDateTime;

        if (v is string s && s.Length > 0)
        {
            if (assumeUtc)
            {
                if (DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var d1))
                    return d1.UtcDateTime;

                if (DateTime.TryParse(s, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var d2))
                    return DateTime.SpecifyKind(d2, DateTimeKind.Utc);
            }
            else
            {
                if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d3))
                    return DateTime.SpecifyKind(d3, DateTimeKind.Unspecified);
            }
        }

        return Convert.ToDateTime(v, CultureInfo.InvariantCulture);
    }
}