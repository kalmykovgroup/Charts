using Charts.Api.Application.Contracts.Charts.Dtos;
using Charts.Api.Domain.Contracts.Template;
using System.Data.Common;

namespace Charts.Api.Application.Interfaces
{
    /// <summary>
    /// READ ONLY ридер для сырых точек временного ряда.
    /// Вариант B: SqlFilter содержит только WhereSql; каталог параметров и runtime-значения приходят отдельно.
    /// </summary>
    public interface IRawSeriesReader
    {
        Task<List<RawPointDto>> ReadAsync(
            DbConnection con,
            string entity, 
            string field,
            string timeField,
            DateTime? from,
            DateTime? to,
            IReadOnlyList<FilterClause>? where,
            SqlFilter? sql, 
            IReadOnlyDictionary<string, ReadySqlParam>? paramCatalog,
            int? maxPoints, CancellationToken ct);
         
    }
}
