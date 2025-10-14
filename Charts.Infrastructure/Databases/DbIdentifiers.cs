using System.Text;

namespace Charts.Api.Infrastructure.Databases;



/// <summary>
/// Вспомогательные функции для разбора имён таблиц/сущностей.
/// Поддерживает:
///  - table
///  - schema.table
///  - "Schema"."Table"
///  - "weird.name"."table.with.dots"
/// Если схему не указали — вернёт defaultSchema.
/// </summary>
public static class DbIdentifiers
{
    /// <summary>
    /// Разобрать строку сущности на (schema, table).
    /// </summary>
    /// <param name="input">Имя сущности. Может быть с кавычками и точками.</param>
    /// <param name="defaultSchema">Схема по умолчанию (например, "public" для PostgreSQL, "dbo" для SQL Server).</param>
    public static (string Schema, string Table) ParseEntityName(string input, string defaultSchema = "public")
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Entity name is empty.", nameof(input));

        var parts = SplitByDotOutsideQuotes(input.Trim());

        string schema, table;

        if (parts.Count == 1)
        {
            schema = defaultSchema;
            table = Unquote(parts[0]);
        }
        else
        {
            // Берём последние две части как schema/table.
            // Если частей больше (редко, но бывает из-за точек внутри кавычек),
            // SplitByDotOutsideQuotes уже сделал правильное разбиение.
            schema = Unquote(parts[parts.Count - 2]);
            table = Unquote(parts[parts.Count - 1]);
        }

        if (string.IsNullOrWhiteSpace(table))
            throw new ArgumentException($"Failed to parse table name from '{input}'.");

        if (string.IsNullOrWhiteSpace(schema))
            schema = defaultSchema;

        return (schema, table);
    }

    /// <summary>
    /// Убирает внешние кавычки: "name", `name`, [name] → name.
    /// Двойные кавычки внутри ("" → ") не разворачиваем — это не требуется для текущих задач.
    /// </summary>
    public static string Unquote(string ident)
    {
        if (string.IsNullOrEmpty(ident)) return ident;

        ident = ident.Trim();

        if (ident.Length >= 2)
        {
            // "name"
            if (ident[0] == '"' && ident[^1] == '"')
                return ident.Substring(1, ident.Length - 2);
            // `name` (MySQL)
            if (ident[0] == '`' && ident[^1] == '`')
                return ident.Substring(1, ident.Length - 2);
            // [name] (SQL Server)
            if (ident[0] == '[' && ident[^1] == ']')
                return ident.Substring(1, ident.Length - 2);
        }

        return ident;
    }

    /// <summary>
    /// Разделяет строку по точкам, игнорируя точки внутри двойных кавычек.
    /// Пример:  "weird.name"."table.with.dots"  →  ["\"weird.name\"", "\"table.with.dots\""]
    /// </summary>
    private static List<string> SplitByDotOutsideQuotes(string s)
    {
        var parts = new List<string>();
        if (string.IsNullOrEmpty(s))
        {
            parts.Add(s);
            return parts;
        }

        var inDoubleQuotes = false;
        var sb = new StringBuilder(s.Length);

        for (int i = 0; i < s.Length; i++)
        {
            var ch = s[i];

            if (ch == '"')
            {
                // Поддержка экранированных кавычек "":
                // внутри кавычек две подряд считаем одной и не выходим из режима
                if (inDoubleQuotes && i + 1 < s.Length && s[i + 1] == '"')
                {
                    sb.Append('"');
                    i++; // пропускаем второй символ
                    continue;
                }

                inDoubleQuotes = !inDoubleQuotes;
                sb.Append(ch);
                continue;
            }

            if (ch == '.' && !inDoubleQuotes)
            {
                parts.Add(sb.ToString().Trim());
                sb.Clear();
            }
            else
            {
                sb.Append(ch);
            }
        }

        if (sb.Length > 0)
            parts.Add(sb.ToString().Trim());

        return parts;
    }
}