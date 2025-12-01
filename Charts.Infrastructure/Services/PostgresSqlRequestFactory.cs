// C:\...\Charts.Api.Infrastructure\Services\Test\PostgresSqlRequestFactory.cs

using System.Text;
using Charts.Domain.Contracts.Template;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;

namespace Charts.Infrastructure.Services
{
    public sealed class PostgresSqlRequestFactory : ISqlRequestFactory
    {
        private readonly IWhereCompiler _where;
        private readonly ILogger<PostgresSqlRequestFactory> _log;

        public PostgresSqlRequestFactory(IWhereCompiler where, ILogger<PostgresSqlRequestFactory> log)
        {
            _where = where;
            _log = log;
        }

        public SqlRequest BuildRawPoints(
            string entity, string field, string timeField, TimeColumnKind timeKind,
            long from, long to,
            IReadOnlyList<FilterClause> where, SqlFilter? sql,
            IReadOnlyDictionary<string, ReadySqlParam> catalog,
            int? limit)
        {
            var ps = new List<NpgsqlParameter>();
            var sb = new StringBuilder(256);

            // Базовая часть
            sb.Append("SELECT ")
              .Append(FilterSqlBuilder.Q(timeField)).Append(" AS t, ")
              .Append(FilterSqlBuilder.Q(field)).Append(" AS v ")
              .Append("FROM ").Append(FullName(entity))
              .Append(" WHERE 1=1") // оставляю как у тебя, чтобы diff был минимальным
              .Append(" AND ").Append(FilterSqlBuilder.Q(timeField)).Append(" >= @from")
              .Append(" AND ").Append(FilterSqlBuilder.Q(timeField)).Append(" < @to")
              .Append(" AND ").Append(FilterSqlBuilder.Q(field)).Append(" IS NOT NULL");

            // Временные параметры
            ps.Add(MakeTimeParam("from", timeKind, from));
            ps.Add(MakeTimeParam("to", timeKind, to));

            // === ВАЖНО: вызываем компилятор WHERE (замена старого AppendFilters/SqlSnippet) ===
            var compiled = _where.Compile(where, sql, catalog);
            _log.LogDebug("WHERE compiled: {WhereSql}. WHERE params: {Params}",
                string.IsNullOrEmpty(compiled.Sql) ? "<empty>" : compiled.Sql,
                SqlLog.DescribeParams(compiled.Parameters));

            if (!string.IsNullOrEmpty(compiled.Sql))
            {
                // compiled.Sql = " WHERE ...." → срежем префикс и приклеим через AND (...)
                var trimmed = compiled.Sql.StartsWith(" WHERE ", StringComparison.OrdinalIgnoreCase)
                    ? compiled.Sql.Substring(7)
                    : compiled.Sql;
                sb.Append(" AND (").Append(trimmed).Append(')');
                ps.AddRange(compiled.Parameters);
            }

            // Сортировка/лимит
            sb.Append(" ORDER BY ").Append(FilterSqlBuilder.Q(timeField)).Append(" ASC");

            if (limit is > 0)
            {
                sb.Append(" LIMIT @limit");
                ps.Add(new NpgsqlParameter("limit", NpgsqlDbType.Integer) { Value = limit.Value });
            }

            var sqlText = sb.ToString();

            // Отладка: печать SQL/параметров/expanded
            _log.LogDebug("BuildRawPoints SQL=\n{Sql}\nPARAMS: {Params}\nExpanded:\n{Expanded}",
                sqlText, SqlLog.DescribeParams(ps), SqlLog.ExpandSqlForLog(sqlText, ps));

            // Жёсткие проверки
            var expanded = SqlLog.ExpandSqlForLog(sqlText, ps);
            if (expanded.Contains("{{"))
                throw new InvalidOperationException($"Dangling placeholders in SQL: {expanded}");
            if (ps.Any(p => p.NpgsqlDbType == NpgsqlDbType.Jsonb && expanded.Contains(" = @" + p.ParameterName)))
                throw new InvalidOperationException($"Jsonb parameter used in '=' predicate: {SqlLog.DescribeParams(ps)}");

            return new SqlRequest(sqlText, ps);
        }

        public SqlRequest BuildEdgeTime(
            string entity, string timeField, bool isMax,
            IReadOnlyList<FilterClause> where, SqlFilter? sql,
            IReadOnlyDictionary<string, ReadySqlParam> catalog)
        {
            var ps = new List<NpgsqlParameter>();
            var sb = new StringBuilder(128);

            sb.Append("SELECT ").Append(isMax ? "MAX(" : "MIN(")
              .Append(FilterSqlBuilder.Q(timeField)).Append(")")
              .Append(" FROM ").Append(FullName(entity));

            var compiled = _where.Compile(where, sql, catalog);
            if (!string.IsNullOrEmpty(compiled.Sql))
            {
                sb.Append(compiled.Sql);
                ps.AddRange(compiled.Parameters);
            }

            var sqlText = sb.ToString();
            _log.LogDebug("BuildEdgeTime({MaxMin}) SQL=\n{Sql}\nPARAMS: {Params}\nExpanded:\n{Expanded}",
                isMax ? "MAX" : "MIN", sqlText, SqlLog.DescribeParams(ps), SqlLog.ExpandSqlForLog(sqlText, ps));

            return new SqlRequest(sqlText, ps);
        }

        private static NpgsqlParameter MakeTimeParam(string name, TimeColumnKind kind, long timeMs)
        {
            // Всегда конвертируем в UTC для PostgreSQL 
            var dto = DateTimeOffset.FromUnixTimeMilliseconds(timeMs);
            var utcDateTime = dto.UtcDateTime;
             

            if (kind == TimeColumnKind.Timestamptz)
            {
                // Для timestamptz используем DateTimeOffset с UTC
                return new NpgsqlParameter(name, NpgsqlDbType.TimestampTz)
                {
                    Value = new DateTimeOffset(utcDateTime, TimeSpan.Zero)
                };
            }
            else
            {
                // Для timestamp without time zone используем DateTime
                return new NpgsqlParameter(name, NpgsqlDbType.Timestamp)
                {
                    Value = utcDateTime
                };
            }
        } 

        private static string FullName(string entity)
        {
            var i = entity.IndexOf('.');
            if (i < 0) return FilterSqlBuilder.Q(entity.Trim());
            var schema = entity[..i].Trim();
            var table = entity[(i + 1)..].Trim();
            return $"{FilterSqlBuilder.Q(schema)}.{FilterSqlBuilder.Q(table)}";
        }
    }
}
