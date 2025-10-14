using Charts.Api.Application.Contracts.Charts.Dtos;
using Charts.Api.Domain.Contracts.Template.Dtos;
using Charts.Api.Application.Contracts.Metadata.Dtos;
using Charts.Api.Domain.Contracts.Template;
using System.Data.Common;

namespace Charts.Api.Infrastructure.Services;
public interface IChartQueryPlanner
{
    Task<(bool Success, ChartQueryPlan? Plan)> BuildPlanAsync(
        DbConnection con,
        ResolvedCharReqTemplate tpl,
        int? px,
        int? maxPoints,
        int? bucketMs,
        CancellationToken ct);

    int PickBucketMilliseconds(long from, long to, int px);
}

public sealed record ChartQueryPlan(
    string Entity,
    string TimeField,
    FieldDto[] Fields,
    long From,
    long To,
    IReadOnlyList<FilterClause> Where,
    SqlFilter? Sql,
    IReadOnlyDictionary<string, ReadySqlParam> ParamCatalog,
    int? Px,
    int BucketMilliseconds,
    int? MaxPoints);

public enum TimeColumnKind { Timestamp, Timestamptz }

public interface ITimeColumnInspector
{
    Task<TimeColumnKind> GetKindAsync(DbConnection con, string entity, string timeField, CancellationToken ct);
}

public sealed record SqlRequest(string Sql, IReadOnlyList<Npgsql.NpgsqlParameter> Parameters);

public interface ISqlRequestFactory
{
    SqlRequest BuildRawPoints(string entity, string field, string timeField, TimeColumnKind timeKind,
                                long from, long to,
                                IReadOnlyList<FilterClause> where, SqlFilter? sql,
                                IReadOnlyDictionary<string, ReadySqlParam> catalog,
                                int? limit);

    SqlRequest BuildEdgeTime(string entity, string timeField, bool isMax,
                                IReadOnlyList<FilterClause> where, SqlFilter? sql,
                                IReadOnlyDictionary<string, ReadySqlParam> catalog);
}

public interface IRawDataExecutor
{
    Task<List<RawPointDto>> ExecutePointsAsync(DbConnection con, SqlRequest request, CancellationToken ct);
    Task<long?> ExecuteEdgeTimeAsync(DbConnection con, SqlRequest request, CancellationToken ct);
}
 

