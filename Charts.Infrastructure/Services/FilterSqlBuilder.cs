using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Charts.Domain.Contracts.Metadata.Dtos;
using Charts.Domain.Contracts.Template;
using Charts.Domain.Contracts.Types;
using Npgsql;
using NpgsqlTypes;

public static class FilterSqlBuilder
{
     
    private static readonly Regex KeyRx = new(@"{{\s*([\w:.\-]+)\s*}}", RegexOptions.Compiled);
    // при желании усили: добавь DROP/TRUNCATE и т.п.
    private static readonly Regex DangerRx = new(@"(--|/\*|\*/|;)", RegexOptions.Compiled);

    // ============== PUBLIC API ==============

    /// <summary>
    /// Единая обработка UI-фильтров (конструктор: поле/оператор/значение).
    /// Плейсхолдеры {{key}} → типизированные параметры по каталогу/существующим параметрам.
    /// Литералы типизируются по FieldDto.
    /// </summary>
    public static void AppendFilters(
        StringBuilder sb,
        IReadOnlyList<FilterClause>? where,
        List<NpgsqlParameter> ps,
        IReadOnlyDictionary<string, ReadySqlParam> paramCatalog)
    {
        if (where is null || where.Count == 0) return;

        int idx = 0;

        foreach (var c in where)
        {
            if (c.Field is null) throw new InvalidOperationException("FilterClause.Field is null");
            var fieldName = c.Field.Name ?? throw new InvalidOperationException("FilterClause.Field.Name is empty");
            var sel = Q(fieldName);

            // создаёт/типизирует параметр под одно значение (учитывает {{key}} и FieldDto)
            NpgsqlParameter MakeParam(object value)
            {
                // 1) чистый плейсхолдер {{key}} → биндинг через каталог/рантайм (или по уже существующему @key)
                if (value is string s)
                {
                    var m = KeyRx.Match(s);
                    if (m.Success && m.Value.Length == s.Length)
                    {
                        var key = m.Groups[1].Value;
                        return BindKeyParam(ps, key, paramCatalog);
                    }
                }

                // 2) литерал → тип по FieldDto (IsTime?/IsNumeric?/Type)
                NpgsqlDbType? forced = ForceTypeFromField(c.Field);
                if (!forced.HasValue && value is string sv)
                    forced = GuessDbTypeFromLiteral(sv);

                var name = "p" + (++idx).ToString();
                var coerced = forced.HasValue ? CoerceByDbType(forced.Value, value) : value;

                var p = forced.HasValue
                    ? new NpgsqlParameter(name, forced.Value) { Value = coerced ?? DBNull.Value }
                    : new NpgsqlParameter(name, coerced ?? DBNull.Value);
                 
                ps.Add(p);
                return p;
            }

            switch (c.Op)
            {
                case FilterOp.IsNull:
                    sb.Append(" AND (").Append(sel).Append(" IS NULL)");
                    break;

                case FilterOp.NotNull:
                    sb.Append(" AND (").Append(sel).Append(" IS NOT NULL)");
                    break;

                case FilterOp.Between:
                    {
                        var list = ToList(c.Value);
                        if (list.Count != 2)
                            throw new NotSupportedException("Between requires exactly 2 values.");
                        var p1 = MakeParam(list[0]);
                        var p2 = MakeParam(list[1]);
                        sb.Append(" AND (").Append(sel)
                          .Append(" BETWEEN @").Append(p1.ParameterName)
                          .Append(" AND @").Append(p2.ParameterName).Append(')');
                        break;
                    }

                case FilterOp.In:
                case FilterOp.Nin:
                    {
                        var list = ToList(c.Value);
                        if (list.Count == 0) { sb.Append(" AND (FALSE)"); break; } // пустой IN → заведомо ложь
                        var names = list.Select(v => "@" + MakeParam(v).ParameterName);
                        sb.Append(" AND (").Append(sel).Append(c.Op == FilterOp.Nin ? " NOT IN (" : " IN (")
                          .Append(string.Join(", ", names)).Append("))");
                        break;
                    }

                case FilterOp.Like:
                case FilterOp.ILike:
                    {
                        var p = MakeParam(c.Value);
                        sb.Append(" AND (").Append(sel).Append(c.Op == FilterOp.ILike ? " ILIKE " : " LIKE ")
                          .Append('@').Append(p.ParameterName).Append(')');
                        break;
                    }

                case FilterOp.Eq:
                case FilterOp.Ne:
                case FilterOp.Gt:
                case FilterOp.Gte:
                case FilterOp.Lt:
                case FilterOp.Lte:
                    {
                        var p = MakeParam(c.Value);
                        var op = c.Op switch
                        {
                            FilterOp.Eq => "=",
                            FilterOp.Ne => "<>",
                            FilterOp.Gt => ">",
                            FilterOp.Gte => ">=",
                            FilterOp.Lt => "<",
                            FilterOp.Lte => "<=",
                            _ => throw new NotSupportedException()
                        };
                        sb.Append(" AND (").Append(sel).Append(' ').Append(op).Append(' ')
                          .Append('@').Append(p.ParameterName).Append(')');
                        break;
                    }

                default:
                    throw new NotSupportedException($"Operator {c.Op} is not supported");
            }
        }
    }


        private static NpgsqlDbType? GuessDbTypeFromLiteral(string s)
    {
        var str = s.Trim();

        // ISO Date: 2025-08-29
        if (Regex.IsMatch(str, @"^\d{4}-\d{2}-\d{2}$"))
            return NpgsqlDbType.Date;

        // ISO DateTime: 2025-08-29T15:00:00(.fff)?
        var m = Regex.Match(str, @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d+)?(Z|[+\-]\d{2}:\d{2})?$");
        if (m.Success)
        {
            // есть Z или смещение -> timestamptz, иначе — timestamp
            if (str.EndsWith("Z", StringComparison.OrdinalIgnoreCase) ||
                Regex.IsMatch(str, @"[+\-]\d{2}:\d{2}$"))
                return NpgsqlDbType.TimestampTz;
            return NpgsqlDbType.Timestamp;
        }

        return null; // иначе не угадываем
    }

    /// <summary>Обратная совместимость: без каталога/значений.</summary>
    public static void AppendFilters(
        StringBuilder sb,
        IReadOnlyList<FilterClause>? where,
        List<NpgsqlParameter> ps)
    => AppendFilters(sb, where, ps, new Dictionary<string, ReadySqlParam>(StringComparer.OrdinalIgnoreCase));

    /// <summary>
    /// Строгая обработка WhereSql: ищем все {{key}}, создаём/обновляем типизированные параметры @key и подставляем их.
    /// </summary>
    public static void AppendSqlSnippetStrict(
        StringBuilder sb,
        string whereSql,
        IReadOnlyDictionary<string, ReadySqlParam> catalog, 
        List<NpgsqlParameter> ps)
    {
        if (string.IsNullOrWhiteSpace(whereSql)) return;
        if (DangerRx.IsMatch(whereSql))
            throw new InvalidOperationException("SqlFilter contains forbidden tokens");

        var keys = KeyRx.Matches(whereSql);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match m in keys)
        {
            var key = m.Groups[1].Value;
            if (!seen.Add(key)) continue;
            BindKeyParam(ps, key, catalog);
        }

        var sqlWithParams = KeyRx.Replace(whereSql, m => "@" + m.Groups[1].Value);
        sb.Append(" AND (").Append(sqlWithParams).Append(')');
    }

    // ============== CORE BINDING/COERCION ==============

    /// <summary>
    /// Биндинг {{key}} → @key: если @key уже существует (например, системные @from/@to), приводим значение по типу существующего параметра.
    /// Иначе используем тип из каталога (SqlParam.Type / SqlParamType).
    /// </summary>
    private static NpgsqlParameter BindKeyParam(
        List<NpgsqlParameter> ps,
        string key,
        IReadOnlyDictionary<string, ReadySqlParam> catalog)
    {
        if (!catalog.TryGetValue(key, out ReadySqlParam? def))
            throw new InvalidOperationException($"Sql key '{key}' is not defined in template.Params.");
         

        var raw = def.Value;

        if (def.Required == true && raw is null)
            throw new InvalidOperationException($"Required parameter '{key}' is missing.");

        // уже есть такой параметр? → используем его тип
        var existing = ps.FirstOrDefault(p => string.Equals(p.ParameterName, key, StringComparison.OrdinalIgnoreCase));

        // тип из каталога (string или enum → string)
        var typeFromCatalog = ((def.Field?.Type ?? def.Field?.SqlParamType.ToString()) ?? "").ToLowerInvariant();

        if (existing != null)
        {
            if (!string.IsNullOrEmpty(typeFromCatalog))
            {
                var (t, coerced) = MapAndCoerce(typeFromCatalog, raw);
                if (t.HasValue) existing.NpgsqlDbType = t.Value;
                existing.Value = coerced ?? DBNull.Value;
            }
            else
            {
                existing.Value = existing.NpgsqlDbType switch
                {
                    NpgsqlDbType.TimestampTz => CoerceDateTime(raw, true) ?? DBNull.Value,
                    NpgsqlDbType.Timestamp => CoerceDateTime(raw, false) ?? DBNull.Value,
                    NpgsqlDbType.Date => CoerceDateOnly(raw) ?? DBNull.Value,
                    NpgsqlDbType.Integer => CoerceInt(raw) ?? DBNull.Value,
                    NpgsqlDbType.Bigint => CoerceLong(raw) ?? DBNull.Value,
                    NpgsqlDbType.Double => CoerceDouble(raw) ?? DBNull.Value,
                    NpgsqlDbType.Numeric => CoerceDecimal(raw) ?? DBNull.Value,
                    NpgsqlDbType.Boolean => CoerceBool(raw) ?? DBNull.Value,
                    NpgsqlDbType.Uuid => CoerceGuid(raw) ?? DBNull.Value,
                    _ => raw ?? DBNull.Value
                };
            }
            return existing;
        }

        // параметра ещё нет — создаём новый
        if (!string.IsNullOrEmpty(typeFromCatalog))
        {
            var (t, coerced) = MapAndCoerce(typeFromCatalog, raw);
            var p = t.HasValue
                ? new NpgsqlParameter(key, t.Value) { Value = coerced ?? DBNull.Value }
                : new NpgsqlParameter(key, coerced ?? DBNull.Value);
            ps.Add(p);
            return p;
        }
        else
        {
            // нет типа вообще: создаём без типа (text). Лучше в шаблоне задать Type.
            var p = new NpgsqlParameter(key, raw ?? DBNull.Value);
            ps.Add(p);
            return p;
        }
    }

    /// <summary>Тип для литерала по FieldDto.</summary>
    private static NpgsqlDbType? ForceTypeFromField(FieldDto field)
    {
        // 1) Пытаемся распознать по строковому типу поля из БД (самый надёжный путь)
        var t = (field?.Type ?? "").ToLowerInvariant().Trim();

        if (t.Contains("with time zone") || t.Contains("timestamptz") || t.Contains("tz"))
            return NpgsqlDbType.TimestampTz;
        if (t.Contains("timestamp"))
            return NpgsqlDbType.Timestamp;
        if (t.Contains("date"))
            return NpgsqlDbType.Date;

        if (t.Contains("uuid")) return NpgsqlDbType.Uuid;
        if (t.Contains("bool")) return NpgsqlDbType.Boolean;
        if (t.Contains("bigint") || t.Contains("int8")) return NpgsqlDbType.Bigint;
        if (t.Contains("double") || t.Contains("real") || t.Contains("float8") || t.Contains("float4"))
            return NpgsqlDbType.Double;
        if (t.Contains("numeric") || t.Contains("decimal"))
            return NpgsqlDbType.Numeric;
        if (t.Contains("int")) return NpgsqlDbType.Integer;

        // 2) Иначе — мягкие подсказки от флагов (если заданы)
        if (field?.IsTime == true) return NpgsqlDbType.Timestamp; // дефолт без tz, если строка типа пустая
        if (field?.IsNumeric == true) return NpgsqlDbType.Integer;

        return null;
    }


    private static (NpgsqlDbType?, object?) MapAndCoerce(string? type, object? value)
        => (type ?? "").ToLowerInvariant() switch
        {
            "timestamptz" => (NpgsqlDbType.TimestampTz, CoerceDateTime(value, true)),
            "timestamp" => (NpgsqlDbType.Timestamp, CoerceDateTime(value, false)),
            "date" => (NpgsqlDbType.Date, CoerceDateOnly(value)),
            "int" => (NpgsqlDbType.Integer, CoerceInt(value)),
            "bigint" => (NpgsqlDbType.Bigint, CoerceLong(value)),
            "double" => (NpgsqlDbType.Double, CoerceDouble(value)),
            "numeric" => (NpgsqlDbType.Numeric, CoerceDecimal(value)),
            "bool" => (NpgsqlDbType.Boolean, CoerceBool(value)),
            "uuid" => (NpgsqlDbType.Uuid, CoerceGuid(value)),
            "text" or "string" => (NpgsqlDbType.Text, value?.ToString()),
            _ => (null, value)
        };

    private static object? CoerceByDbType(NpgsqlDbType dbt, object? v)
        => dbt switch
        {
            NpgsqlDbType.TimestampTz => CoerceDateTime(v, true),
            NpgsqlDbType.Timestamp => CoerceDateTime(v, false),
            NpgsqlDbType.Date => CoerceDateOnly(v),
            NpgsqlDbType.Integer => CoerceInt(v),
            NpgsqlDbType.Bigint => CoerceLong(v),
            NpgsqlDbType.Double => CoerceDouble(v),
            NpgsqlDbType.Numeric => CoerceDecimal(v),
            NpgsqlDbType.Boolean => CoerceBool(v),
            NpgsqlDbType.Uuid => CoerceGuid(v),
            _ => v
        };

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
        return v;
    }

    private static object? CoerceDateOnly(object? v)
    {
        if (v is null or DBNull) return null;
        if (v is DateTime dt) return dt.Date;
        if (v is string s && DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
            return d.Date;
        return v;
    }

    private static object? CoerceInt(object? v)
        => v is int ? v : (v is string s && int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i) ? i : v);

    private static object? CoerceLong(object? v)
        => v is long ? v : (v is string s && long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var l) ? l : v);

    private static object? CoerceDouble(object? v)
        => v is double ? v : (v is string s && double.TryParse(s.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : v);

    private static object? CoerceDecimal(object? v)
        => v is decimal ? v : (v is string s && decimal.TryParse(s.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var d) ? d : v);

    private static object? CoerceBool(object? v)
    {
        if (v is bool) return v;
        if (v is string s)
        {
            s = s.Trim().ToLowerInvariant();
            if (s is "true" or "1" or "yes" or "y" or "on") return true;
            if (s is "false" or "0" or "no" or "n" or "off") return false;
        }
        return v;
    }

    private static object? CoerceGuid(object? v)
        => v is Guid ? v : (v is string s && Guid.TryParse(s, out var g) ? g : v);

    private static List<object?> ToList(object? v)
    {
        if (v is null or DBNull) return new();
        if (v is IEnumerable e && v is not string)
        {
            var list = new List<object?>();
            foreach (var it in e) list.Add(it);
            return list;
        }
        return new() { v };
    }




     

    private static readonly IReadOnlyDictionary<string, object?> EmptyRuntime = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

    public static readonly Regex NameRx = new("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled);
    public static string Q(string ident)
    {
        if (string.IsNullOrWhiteSpace(ident) || !NameRx.IsMatch(ident))
            throw new ArgumentException($"Invalid SQL identifier: {ident}");
        return '"' + ident + '"';
    }
}
