using Charts.Api.Domain.Contracts.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Charts.Api.Infrastructure.Utils
{
    public static class SqlParamTypeMapper
    {
        /// <summary>
        /// Пытается привести сырой тип БД к нормализованному SqlParamType для UI/фильтров.
        /// Возвращает null, если тип распознать нельзя (лучше оставить на усмотрение клиента).
        /// </summary>
        public static SqlParamType? MapDbTypeToParam(DbProviderType provider, string? dbTypeRaw)
        {
            if (string.IsNullOrWhiteSpace(dbTypeRaw))
                return null;

            var t0 = Normalize(dbTypeRaw);

            // Вендор-специфичные разборы
            switch (provider)
            {
                case DbProviderType.PostgreSql:
                    return MapPostgres(t0) ?? MapGeneric(t0);

                case DbProviderType.SqlServer:
                    return MapSqlServer(t0) ?? MapGeneric(t0);

                case DbProviderType.MySql:
                    return MapMySql(t0) ?? MapGeneric(t0);

                case DbProviderType.Sqlite:
                    return MapSqlite(t0) ?? MapGeneric(t0);

                case DbProviderType.Oracle:
                    return MapOracle(t0) ?? MapGeneric(t0);

                default:
                    // неизвестный провайдер — применим общие эвристики
                    return MapGeneric(t0);
            }
        }

        // -------------------- Helpers --------------------

        private static string Normalize(string s)
        {
            s = s.Trim().ToLowerInvariant();

            // убираем размерности: varchar(50), numeric(10,2), raw(16), char(36) и т.п.
            s = Regex.Replace(s, @"\s*\(\s*[\d\s,]+\s*\)", string.Empty);

            // убираем массивность: integer[], uuid[], ...
            s = s.EndsWith("[]") ? s[..^2] : s;

            // схлопываем повторные пробелы
            s = Regex.Replace(s, @"\s+", " ");

            // унификация общих синонимов
            s = s
                .Replace("character varying", "varchar")
                .Replace("double precision", "double")
                .Replace("without time zone", "")     // "timestamp without time zone" -> "timestamp"
                .Replace("with time zone", "tz");     // "timestamp with time zone"    -> "timestamp tz"

            return s;
        }

        private static SqlParamType? MapPostgres(string t)
        {
            // время/даты
            if (t is "timestamptz" or "timestamp tz" or "timestampz") return SqlParamType.Timestamptz;
            if (t.StartsWith("timestamp")) return SqlParamType.Timestamp;
            if (t is "date") return SqlParamType.Date;

            // логика
            if (t is "boolean" or "bool") return SqlParamType.Bool;

            // uuid
            if (t is "uuid") return SqlParamType.Uuid;

            // числа
            if (t is "bigint" or "int8") return SqlParamType.Bigint;
            if (t is "integer" or "int" or "int4" or "smallint" or "int2") return SqlParamType.Int;
            if (t is "double" or "float8" or "real" or "float4" or "numeric" or "decimal") return SqlParamType.Double;

            // строки/прочее текстовое
            if (t is "text" or "varchar" or "char" or "bpchar" or "citext") return SqlParamType.Text;

            // json/jsonb — чаще как текстовый параметр
            if (t is "json" or "jsonb") return SqlParamType.Text;

            return null;
        }

        private static SqlParamType? MapSqlServer(string t)
        {
            // время/даты
            if (t is "datetimeoffset") return SqlParamType.Timestamptz;
            if (t is "datetime2" or "datetime" or "smalldatetime") return SqlParamType.Timestamp;
            if (t is "date") return SqlParamType.Date;

            // логика
            if (t is "bit" or "boolean" or "bool") return SqlParamType.Bool;

            // uuid
            if (t is "uniqueidentifier") return SqlParamType.Uuid;

            // числа
            if (t is "bigint") return SqlParamType.Bigint;
            if (t is "int" or "smallint" or "tinyint") return SqlParamType.Int;
            if (t is "float" or "real" or "decimal" or "numeric" or "money" or "smallmoney") return SqlParamType.Double;

            // строки
            if (t.Contains("varchar") || t.Contains("char") || t is "text" or "ntext") return SqlParamType.Text;

            return null;
        }

        private static SqlParamType? MapMySql(string t)
        {
            // время/даты
            if (t is "timestamp" or "datetime") return SqlParamType.Timestamp;
            if (t is "date") return SqlParamType.Date;

            // логика (в MySQL boolean → tinyint(1), но для UI это флажок)
            if (t is "boolean" or "bool") return SqlParamType.Bool;

            // uuid (в MySQL часто CHAR(36) / BINARY(16); если явно "uuid" — мапим)
            if (t is "uuid") return SqlParamType.Uuid;

            // числа
            if (t is "bigint") return SqlParamType.Bigint;
            if (t is "int" or "integer" or "mediumint" or "smallint" or "tinyint") return SqlParamType.Int;
            if (t is "double" or "float" or "decimal" or "numeric") return SqlParamType.Double;

            // строки
            if (t.Contains("varchar") || t.Contains("char") || t.Contains("text")) return SqlParamType.Text;

            // json
            if (t is "json") return SqlParamType.Text;

            return null;
        }

        private static SqlParamType? MapSqlite(string t)
        {
            // SQLite хранит "аффинности", поэтому применяем простые эвристики
            if (t.Contains("int")) return SqlParamType.Int;      // bigint/… тоже сюда
            if (t.Contains("real") || t.Contains("double") || t.Contains("numeric") || t.Contains("decimal"))
                return SqlParamType.Double;

            if (t.Contains("datetime") || t.Contains("timestamp")) return SqlParamType.Timestamp;
            if (t is "date") return SqlParamType.Date;

            if (t.Contains("bool")) return SqlParamType.Bool;
            if (t.Contains("uuid")) return SqlParamType.Uuid;

            // всё остальное — текст
            return SqlParamType.Text;
        }

        private static SqlParamType? MapOracle(string t)
        {
            // время/даты
            if (t is "timestamp with time zone" or "timestamptz" or "timestamp tz") return SqlParamType.Timestamptz;
            if (t.StartsWith("timestamp")) return SqlParamType.Timestamp;

            // В Oracle DATE содержит время до секунд — для UI чаще ближе к Timestamp
            if (t is "date") return SqlParamType.Timestamp;

            // числа: NUMBER[(p[,s])]
            if (t is "binary_float" or "binary_double") return SqlParamType.Double;
            if (t is "number")
                return SqlParamType.Double; // без точной p,s надёжнее отдать как Double
                                            // (если хочется, можно парсить p,s из сырой строки до Normalize)

            // строки
            if (t is "char" or "nchar" || t.Contains("varchar")) return SqlParamType.Text;
            if (t is "clob" or "nclob") return SqlParamType.Text;

            // UUID: часто RAW(16) или CHAR(36) — без контрактной оговорки лучше не мапить
            if (t is "uuid") return SqlParamType.Uuid;

            return null;
        }

        /// <summary>
        /// Общие эвристики на случай неизвестного провайдера/типа.
        /// </summary>
        private static SqlParamType? MapGeneric(string t)
        {
            if (t.Contains("tz")) return SqlParamType.Timestamptz;
            if (t.Contains("timestamp") || t.Contains("datetime")) return SqlParamType.Timestamp;
            if (t == "date") return SqlParamType.Date;

            if (t.Contains("bool")) return SqlParamType.Bool;
            if (t.Contains("uuid") || t == "guid") return SqlParamType.Uuid;

            if (t.Contains("bigint")) return SqlParamType.Bigint;
            if (t.Contains("int")) return SqlParamType.Int;

            if (t.Contains("double") || t.Contains("float") || t.Contains("real") || t.Contains("numeric") || t.Contains("decimal"))
                return SqlParamType.Double;

            if (t.Contains("char") || t.Contains("text") || t.Contains("varchar"))
                return SqlParamType.Text;

            if (t is "json" or "jsonb") return SqlParamType.Text;

            return null;
        }
    }
}
