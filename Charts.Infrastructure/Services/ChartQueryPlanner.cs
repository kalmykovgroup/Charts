using System.Data.Common;
using Charts.Domain.Contracts.Metadata.Dtos;
using Charts.Domain.Contracts.Template;
using Charts.Domain.Contracts.Template.Dtos;
using Charts.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Charts.Infrastructure.Services;

/// <summary>
/// Планировщик: собирает входные данные для построения запроса (from/to, where, params, bucketMilliseconds и т.п.).
/// ВНИМАНИЕ: Планировщик НЕ узнаёт тип временной колонки (timestamp/timestamptz) и НЕ исполняет SQL —
/// этим занимается ChartDataService (через ITimeColumnInspector и IRawDataExecutor).
/// </summary> 
public sealed class ChartQueryPlanner : IChartQueryPlanner
{
    private readonly ISqlRequestFactory _sqlFactory;
    private readonly IRawDataExecutor _executor;
    private readonly ILogger<ChartQueryPlanner> _log;
    private readonly ChartBucketingOptions _opt;

    public ChartQueryPlanner(
        ISqlRequestFactory sqlFactory,
        IRawDataExecutor executor,
        ILogger<ChartQueryPlanner> log,
        IOptions<ChartBucketingOptions> opt
    )
    {
        _sqlFactory = sqlFactory;
        _executor = executor;
        _log = log;
        _opt = opt.Value;
    }

    public async Task<(bool Success, ChartQueryPlan? Plan)> BuildPlanAsync(
    DbConnection con,
    ResolvedCharReqTemplate tpl,
    int? px,
    int? maxPoints,
    int? bucketMs,
    CancellationToken ct)
    {
        try
        {
            // Каталог параметров для плейсхолдеров {{key}}
            var catalog = BuildCatalog(tpl);

            // WHERE может быть null — нормализуем
            var where = tpl.Where ?? [];

            // Определяем границы времени: берём из tpl или запрашиваем из БД
            long? minTime = tpl.ResolvedFromMs;
            long? maxTime = tpl.ResolvedToMs;

            // Запрашиваем MIN только если ResolvedFromMs не задан
            if (!minTime.HasValue)
            {
                var minReq = _sqlFactory.BuildEdgeTime(
                    tpl.Entity.Name,
                    tpl.TimeField.Name,
                    isMax: false,
                    where,
                    tpl.Sql,
                    catalog);

                minTime = await _executor.ExecuteEdgeTimeAsync(con, minReq, ct);

                _log.LogDebug("MIN time from database: {MinTime}",
                    minTime.HasValue
                        ? DateTimeOffset.FromUnixTimeMilliseconds(minTime.Value).ToString("O")
                        : "NULL");
            }

            // Запрашиваем MAX только если ResolvedToMs не задан
            if (!maxTime.HasValue)
            {
                var maxReq = _sqlFactory.BuildEdgeTime(
                    tpl.Entity.Name,
                    tpl.TimeField.Name,
                    isMax: true,
                    where,
                    tpl.Sql,
                    catalog);

                maxTime = await _executor.ExecuteEdgeTimeAsync(con, maxReq, ct);

                _log.LogDebug("MAX time from database: {MaxTime}",
                    maxTime.HasValue
                        ? DateTimeOffset.FromUnixTimeMilliseconds(maxTime.Value).ToString("O")
                        : "NULL");
            }

            // Если не удалось получить границы — данных нет, возвращаем false
            if (!minTime.HasValue || !maxTime.HasValue)
            {
                var message = $"No data found in '{tpl.Entity.Name}' matching the specified filters. " +
                             $"MIN={minTime}, MAX={maxTime}";

                _log.LogInformation("BuildPlanAsync failed: {Message}", message);

                return (false, null);
            }

            // Валидация: from должен быть меньше to
            if (minTime.Value >= maxTime.Value)
            {
                var message = $"Invalid time range: from ({minTime.Value}) must be less than to ({maxTime.Value}). " +
                             $"From: {DateTimeOffset.FromUnixTimeMilliseconds(minTime.Value):O}, " +
                             $"To: {DateTimeOffset.FromUnixTimeMilliseconds(maxTime.Value):O}";

                _log.LogWarning("BuildPlanAsync failed: {Message}", message);

                return (false, null);
            }

            _log.LogInformation(
                "Time range resolved: from={From} to={To}, span={SpanMs}ms ({SpanDays:F2} days)",
                DateTimeOffset.FromUnixTimeMilliseconds(minTime.Value).ToString("O"),
                DateTimeOffset.FromUnixTimeMilliseconds(maxTime.Value).ToString("O"),
                maxTime.Value - minTime.Value,
                (maxTime.Value - minTime.Value) / (1000.0 * 60 * 60 * 24));

            // Определяем размер бакета
            int? pxClamped = px.HasValue ? ClampPx(px.Value) : null;

            int bucketMilliseconds;
            if (bucketMs.HasValue && bucketMs.Value > 0)
            {
                // Если bucketMs задан явно — используем его
                bucketMilliseconds = bucketMs.Value;
                _log.LogDebug("Using explicit bucket size: {BucketMs}ms", bucketMilliseconds);
            }
            else if (pxClamped.HasValue)
            {
                // Автоматический подбор по px
                bucketMilliseconds = PickBucketMilliseconds(minTime.Value, maxTime.Value, pxClamped.Value);
                _log.LogDebug("Auto-calculated bucket size for px={Px}: {BucketMs}ms", pxClamped.Value, bucketMilliseconds);
            }
            else
            {
                // Нет ни bucketMs, ни px — используем 0 (без агрегации)
                bucketMilliseconds = 0;
                _log.LogDebug("No bucket size specified, using raw data (bucket=0)");
            }

            // Поля для выборки
            var fields = tpl.SelectedFields ?? Array.Empty<FieldDto>();

            if (fields.Length == 0)
            {
                _log.LogWarning("No fields selected for chart query");
            }

            var plan = new ChartQueryPlan(
                Entity: tpl.Entity.Name,
                TimeField: tpl.TimeField.Name,
                Fields: fields,
                From: minTime.Value,
                To: maxTime.Value,
                Where: where,
                Sql: tpl.Sql,
                ParamCatalog: catalog,
                Px: pxClamped,
                BucketMilliseconds: bucketMilliseconds,
                MaxPoints: maxPoints
            );

            return (true, plan);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Unexpected error in BuildPlanAsync for entity {Entity}", tpl.Entity.Name);
            return (false, null);
        }
    }

    // ===== helpers =====

    private static IReadOnlyDictionary<string, ReadySqlParam> BuildCatalog(ResolvedCharReqTemplate tpl) =>
        (tpl.Params ?? new List<ReadySqlParam>(0))
        .GroupBy(p => p.Key, StringComparer.OrdinalIgnoreCase)
        .ToDictionary(g => g.Key, g => g.Last(), StringComparer.OrdinalIgnoreCase);

    private static int ClampPx(int px) => Math.Min(4000, Math.Max(10, px));

    public int PickBucketMilliseconds(long from, long to, int px)  // <-- Изменено название
    {

         var spanMs = Math.Max(1, to - from);

        // целевые бины: max(MinTargetPoints, px * TargetPointsPerPx)
        var targetPoints = Math.Max(_opt.MinTargetPoints, (int)Math.Round(px * _opt.TargetPointsPerPx));
        targetPoints = Math.Max(1, targetPoints);

        var rough = Math.Max(1, spanMs / targetPoints);

        // 1) сначала пробуем «красивую» сетку из опций (теперь в миллисекундах)
        var nice = _opt.NiceMilliseconds?.OrderBy(x => x).ToArray() ?? Array.Empty<long>();
        foreach (var n in nice)
        {
            if (n >= rough) return (int)n;
        }

        // 2) если rough больше максимального nice:
        if (_opt.EnableWeeklyMultiples)
        {
            // кратные недели до лимита (в миллисекундах)
            const double WeekMs = 7 * 24 * 3600 * 1000d;  // <-- Неделя в миллисекундах
            var mult = (int)Math.Ceiling(rough / WeekMs);
            mult = Math.Max(1, Math.Min(mult, _opt.MaxWeeksMultiple));
            var ms = mult * WeekMs;
            return (int)Math.Round(ms);
        }

        // 3) иначе — просто последний из NiceMilliseconds
        if (nice.Length > 0) return (int)nice[^1];

        // 4) крайний случай: fallback на «неделю» в миллисекундах
        return 604800000;  // <-- 7 дней в миллисекундах
    }

}