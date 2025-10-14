using Charts.Api.Application.Contracts.Metadata.Dtos;
using Charts.Api.Domain.Contracts.Types;
using Charts.Api.Infrastructure.Utils;
using Charts.Api.Domain.Contracts.Template;
using Npgsql;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Charts.Api.Infrastructure.Services;

public sealed class PostgresWhereCompiler : IWhereCompiler
{
    private static readonly Regex PlaceholderWithCast = new(
        @"\{\{\s*(?<key>[A-Za-z_][A-Za-z0-9_]*)\s*(::\s*(?<cast>[A-Za-z0-9_]+))?\s*\}\}",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex ExactPlaceholder = new(
        @"^\s*\{\{\s*(?<key>[A-Za-z_][A-Za-z0-9_]*)\s*\}\}\s*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public CompiledWhere Compile(IReadOnlyList<FilterClause>? where, SqlFilter? sql, IReadOnlyDictionary<string, ReadySqlParam> catalog)
    {
        var sb = new StringBuilder();
        var ps = new List<NpgsqlParameter>();

        if (where is { Count: > 0 })
        {
            foreach (var f in where)
            {
                if (sb.Length > 0) sb.Append(" AND ");
                CompileClause(sb, ps, f, catalog);
            }
        }

        if (sql is { WhereSql: not null } && !string.IsNullOrWhiteSpace(sql.WhereSql))
        {
            if (sb.Length > 0) sb.Append(" AND ");
            AppendSqlSnippetStrict(sb, ps, sql.WhereSql!, catalog);
        }

        var sqlOut = sb.Length > 0 ? " WHERE " + sb.ToString() : string.Empty;
        return new CompiledWhere(sqlOut, ps);
    }

    private static void CompileClause(StringBuilder sb, List<NpgsqlParameter> ps, FilterClause f, IReadOnlyDictionary<string, ReadySqlParam> catalog)
    {
        if (f.Field?.Name is null) throw new ArgumentException("FilterClause.Field.Name is required");
        var col = FilterSqlBuilder.Q(f.Field.Name);

        switch (f.Op)
        {
            case FilterOp.Eq: sb.Append(col).Append(" = ").Append(ParamFromValue(ps, f.Value, catalog, f.Field)); break;
            case FilterOp.Ne: sb.Append(col).Append(" <> ").Append(ParamFromValue(ps, f.Value, catalog, f.Field)); break;
            case FilterOp.Gt: sb.Append(col).Append(" > ").Append(ParamFromValue(ps, f.Value, catalog, f.Field)); break;
            case FilterOp.Gte: sb.Append(col).Append(" >= ").Append(ParamFromValue(ps, f.Value, catalog, f.Field)); break;
            case FilterOp.Lt: sb.Append(col).Append(" < ").Append(ParamFromValue(ps, f.Value, catalog, f.Field)); break;
            case FilterOp.Lte: sb.Append(col).Append(" <= ").Append(ParamFromValue(ps, f.Value, catalog, f.Field)); break;
            case FilterOp.Like: sb.Append(col).Append(" LIKE ").Append(ParamFromValue(ps, f.Value, catalog, f.Field)); break;
            case FilterOp.ILike: sb.Append(col).Append(" ILIKE ").Append(ParamFromValue(ps, f.Value, catalog, f.Field)); break;

            case FilterOp.In:
                sb.Append(col).Append(" = ANY(").Append(ParamFromArrayValue(ps, f.Value, catalog, f.Field)).Append(')');
                break;
            case FilterOp.Nin:
                sb.Append("NOT ").Append(col).Append(" = ANY(").Append(ParamFromArrayValue(ps, f.Value, catalog, f.Field)).Append(')');
                break;

            case FilterOp.IsNull: sb.Append(col).Append(" IS NULL"); break;
            case FilterOp.NotNull: sb.Append(col).Append(" IS NOT NULL"); break;

            case FilterOp.Between:
                {
                    var (p1, p2) = ParamsFromBetweenValue(ps, f.Value, catalog, f.Field);
                    sb.Append(col).Append(" BETWEEN ").Append(p1).Append(" AND ").Append(p2);
                    break;
                }

            default:
                throw new NotSupportedException($"Unknown filter op: {f.Op}");
        }
    }

    // ---------- value → @param (поддержка JsonElement и {{key}}), fallback типизируем по Field.Type ----------
    private static string ParamFromValue(List<NpgsqlParameter> ps, object? value, IReadOnlyDictionary<string, ReadySqlParam> catalog, FieldDto field)
    {
        var un = Unwrap(value);

        if (un is string s)
        {
            var m = ExactPlaceholder.Match(s);
            if (m.Success)
            {
                var key = m.Groups["key"].Value;
                if (!catalog.TryGetValue(key, out var rp))
                    throw new InvalidOperationException($"Param '{key}' not found in template Params.");
                var p = PgParamFactory.FromReady(rp);   // тип берётся из rp.Field.Type
                ps.Add(p);
                return "@" + p.ParameterName;
            }
        }

        // не плейсхолдер → создаём временный ReadySqlParam с типом столбца фильтра
        var tempKey = "p" + ps.Count;
        var temp = new ReadySqlParam(tempKey, un!, field, false);
        var auto = PgParamFactory.FromReady(temp);
        ps.Add(auto);
        return "@" + auto.ParameterName;
    }

    // ---------- IN/NIN: Value — массив ИЛИ {{arrKey}}; fallback — массив типизируем по Field.Type ----------
    private static string ParamFromArrayValue(List<NpgsqlParameter> ps, object? value, IReadOnlyDictionary<string, ReadySqlParam> catalog, FieldDto field)
    {
        var un = Unwrap(value);

        if (un is string s && ExactPlaceholder.IsMatch(s))
        {
            var key = ExactPlaceholder.Match(s).Groups["key"].Value;
            if (!catalog.TryGetValue(key, out var rp))
                throw new InvalidOperationException($"Param '{key}' not found in template Params.");
            var p = PgParamFactory.FromReady(rp); // ожидается Field.Type вида "*[]"
            ps.Add(p);
            return "@" + p.ParameterName;
        }

        // литеральный список → создаём временный ReadySqlParam c Field.Type массива
        if (un is System.Collections.IEnumerable seq && un is not string)
        {
            var tempKey = "p" + ps.Count;
            // гарантируем, что тип — массивный: если Field.Type не заканчивается [] — добавим
            var elemType = field.Type?.EndsWith("[]", StringComparison.OrdinalIgnoreCase) == true
                ? field.Type
                : field.Type is null ? "text[]" : field.Type + "[]";

            var tempField = new FieldDto(field.Name, elemType, sqlParamType: SqlParamTypeMapper.MapDbTypeToParam(DbProviderType.PostgreSql, elemType),  field.IsNumeric, field.IsTime);
            var temp = new ReadySqlParam(tempKey, seq, tempField, false);
            var p = PgParamFactory.FromReady(temp);
            ps.Add(p);
            return "@" + p.ParameterName;
        }

        throw new ArgumentException("In/Nin expect an array Value or a placeholder {{key}} for array param.");
    }

    // ---------- BETWEEN: Value — [a,b] | {from,to} | "a;b" ----------
    private static (string p1, string p2) ParamsFromBetweenValue(List<NpgsqlParameter> ps, object? value, IReadOnlyDictionary<string, ReadySqlParam> catalog, FieldDto field)
    {
        var un = Unwrap(value);

        if (un is System.Collections.IEnumerable seq && un is not string)
        {
            var list = new List<object?>();
            foreach (var it in seq) list.Add(it);
            if (list.Count >= 2)
                return (ParamFromValue(ps, list[0], catalog, field), ParamFromValue(ps, list[1], catalog, field));
        }

        if (un is IDictionary<string, object?> dict)
        {
            dict.TryGetValue("from", out var from);
            dict.TryGetValue("to", out var to);
            if (from is not null && to is not null)
                return (ParamFromValue(ps, from, catalog, field), ParamFromValue(ps, to, catalog, field));
        }

        if (un is string s && s.Contains(';'))
        {
            var parts = s.Split(';', 2);
            return (ParamFromValue(ps, parts[0], catalog, field), ParamFromValue(ps, parts[1], catalog, field));
        }

        throw new ArgumentException("Between expects Value as [a,b] or {from,to} or \"a;b\".");
    }

    // ---------- Sql.WhereSql с {{key}} и {{key::cast}} ----------
    private static void AppendSqlSnippetStrict(StringBuilder sb, List<NpgsqlParameter> ps, string sql, IReadOnlyDictionary<string, ReadySqlParam> catalog)
    {
        int last = 0;
        foreach (Match m in PlaceholderWithCast.Matches(sql))
        {
            if (m.Index > last) sb.Append(sql, last, m.Index - last);

            var key = m.Groups["key"].Value;
            var cast = m.Groups["cast"]?.Success == true ? m.Groups["cast"].Value : null;

            if (!catalog.TryGetValue(key, out var rp))
                throw new InvalidOperationException($"Param '{key}' not found in template Params.");

            var p = PgParamFactory.FromReady(rp);
            ps.Add(p);

            sb.Append('@').Append(p.ParameterName);
            if (!string.IsNullOrWhiteSpace(cast))
                sb.Append("::").Append(cast);

            last = m.Index + m.Length;
        }
        if (last < sql.Length)
            sb.Append(sql, last, sql.Length - last);
    }

    // ---------- Unwrap JsonElement/primitive ----------
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
            _ => je.GetRawText() // массив/объект оставляем как JSON-строку; для IN мы это не используем
        };
    }
}