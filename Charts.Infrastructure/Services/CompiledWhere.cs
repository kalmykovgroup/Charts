using Charts.Domain.Contracts.Template;
using Npgsql;

namespace Charts.Infrastructure.Services
{
    public sealed record CompiledWhere(string Sql, IReadOnlyList<NpgsqlParameter> Parameters);

    public interface IWhereCompiler
    {
        CompiledWhere Compile(
            IReadOnlyList<FilterClause>? where,
            SqlFilter? sql,
            IReadOnlyDictionary<string, ReadySqlParam> catalog);
    }
}
