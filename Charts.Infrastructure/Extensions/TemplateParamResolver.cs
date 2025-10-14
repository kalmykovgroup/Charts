using System.Collections;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Charts.Api.Domain.Contracts.Template;
using Microsoft.Extensions.Logging;

namespace Charts.Api.Infrastructure.Extensions
{
    public static class TemplateParamResolver
    {
        private static readonly Regex KeyRx = new("""{{\s*([\w:.-]+)\s*}}""", RegexOptions.Compiled);
        private static readonly Regex WholeKeyRx = new(@"^\s*{{\s*([\w:.-]+)\s*}}\s*$", RegexOptions.Compiled);

        public static List<FilterClause> ResolveClauses(
            IReadOnlyList<FilterClause> where,
            IReadOnlyDictionary<string, SqlParam> catalog,
            IReadOnlyDictionary<string, object?> runtime,
            ILogger? log = null,
            string? rid = null)
        {

            var list = new List<FilterClause>(where.Count);
            for (int i = 0; i < where.Count; i++)
            {
                var c = where[i];
                var before = Describe(c.Value);
                var resolved = ResolveValue(c.Value, catalog, runtime, log, rid);
                var after = Describe(resolved);

                log?.LogDebug("[RAW {rid}] Resolve[{i}] {field}/{op}: BEFORE(Type={bType},Val={bVal}) -> AFTER(Type={aType},Val={aVal})",
                    rid, i, c.Field, c.Op, before.Type, before.Value, after.Type, after.Value);

                list.Add(c with { Value = resolved });
            }
            return list;
        }

        private static object? ResolveValue(
            object? raw,
            IReadOnlyDictionary<string, SqlParam> catalog,
            IReadOnlyDictionary<string, object?> runtime,
            ILogger? log,
            string? rid)
        {
            // 0) СНАЧАЛА нормализуем (JsonElement/JsonDocument -> .NET)
            var val = ToNet(raw);

            // 1) Чистый плейсхолдер "{{key}}"
            if (val is string s && TryWholeKey(s, out var key))
            {
                var hasRun = runtime.TryGetValue(key, out var rv);
                var hasDef = catalog.TryGetValue(key, out var p) && p.Value is not null;

                if (!hasRun && !hasDef && catalog.TryGetValue(key, out var p2) && p2.Required)
                    throw new InvalidOperationException($"Required parameter '{key}' is missing.");

                var chosen = hasRun ? rv : (hasDef ? catalog[key].Value : null);
                var norm = ToNet(chosen);

                log?.LogDebug("[RAW {rid}] Placeholder '{{{{{key}}}}}' -> {source} value: Type={type}, Val={val}",
                    rid, key, hasRun ? "runtime" : (hasDef ? "default" : "null"),
                    Describe(norm).Type, Describe(norm).Value);

                // ВАЖНО: рекурсивно прогоняем — на случай, если это массив/вложенные плейсхолдеры
                return ResolveValue(norm, catalog, runtime, log, rid);
            }

            // 2) Коллекции/массивы — разворачиваем рекурсивно (каждый элемент)
            if (val is IEnumerable seq && val is not string)
            {
                var acc = new List<object?>();
                foreach (var item in seq)
                    acc.Add(ResolveValue(item, catalog, runtime, log, rid));

                log?.LogDebug("[RAW {rid}] Array resolved -> length={len}", rid, acc.Count);
                return acc.ToArray(); // Between/IN увидит ровно 2/ N значений
            }

            // 3) Строка с вкраплениями "{{key}}" — текстовая подстановка
            if (val is string textWithKeys)
            {
                var replaced = KeyRx.Replace(textWithKeys, mm =>
                {
                    var k = mm.Groups[1].Value;
                    var hasRun = runtime.TryGetValue(k, out var rv);
                    var def = catalog.TryGetValue(k, out var p) ? p.Value : null;
                    var v2 = ToNet(hasRun ? rv : def);
                    return Convert.ToString(v2, CultureInfo.InvariantCulture) ?? string.Empty;
                });

                log?.LogDebug("[RAW {rid}] Interpolate: '{src}' -> '{dst}'", rid, textWithKeys, replaced);
                return replaced;
            }

            // 4) Прочее — уже нормализовано
            return val;
        }

        private static bool TryWholeKey(string s, out string key)
        {
            var m = WholeKeyRx.Match(s);
            key = m.Success ? m.Groups[1].Value : string.Empty;
            return m.Success;
        }

        private static object? ToNet(object? v)
        {
            if (v is null) return null;
            if (v is JsonDocument jd) return FromJson(jd.RootElement);
            if (v is JsonElement je) return FromJson(je);
            return v;
        }

        private static object? FromJson(JsonElement e)
        {
            switch (e.ValueKind)
            {
                case JsonValueKind.Undefined:
                case JsonValueKind.Null: return null;
                case JsonValueKind.True:
                case JsonValueKind.False: return e.GetBoolean();
                case JsonValueKind.String: return e.GetString();
                case JsonValueKind.Number:
                    if (e.TryGetInt64(out var l)) return l;
                    if (e.TryGetDecimal(out var d)) return d;
                    return e.GetDouble();
                case JsonValueKind.Array:
                    {
                        var list = new List<object?>();
                        foreach (var item in e.EnumerateArray())
                            list.Add(FromJson(item));
                        return list.ToArray();
                    }
                case JsonValueKind.Object:
                    return e.GetRawText();
                default:
                    return null;
            }
        }

        private static (string Type, string Value) Describe(object? v)
        {
            if (v is null) return ("<null>", "null");
            if (v is string s) return ("string", s);
            if (v is IEnumerable e && v is not string)
            {
                var list = new List<string>();
                foreach (var it in e) list.Add(Describe(it).Value);
                return ("IEnumerable", $"[{string.Join(", ", list)}]");
            }
            return (v.GetType().Name, Convert.ToString(v, CultureInfo.InvariantCulture) ?? "");
        }
    }
}
