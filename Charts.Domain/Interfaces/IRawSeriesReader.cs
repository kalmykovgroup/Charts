using System.Data.Common;
using Charts.Domain.Contracts.Charts.Dtos;
using Charts.Domain.Contracts.Template;

namespace Charts.Domain.Interfaces
{
    /// <summary>
    /// READ ONLY ����� ��� ����� ����� ���������� ����.
    /// ������� B: SqlFilter �������� ������ WhereSql; ������� ���������� � runtime-�������� �������� ��������.
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
