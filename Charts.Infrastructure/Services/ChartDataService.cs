using Charts.Api.Application.Contracts;
using Charts.Api.Application.Contracts.Charts.Dtos;
using Charts.Api.Application.Contracts.Charts.Requests;
using Charts.Api.Application.Contracts.Charts.Responces;
using Charts.Api.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Charts.Api.Infrastructure.Services
{
    public sealed class ChartDataService : IChartDataService
    {
        private readonly IChartQueryPlanner _planner;
        private readonly ITimeColumnInspector _timeInspector;
        private readonly ISqlRequestFactory _sqlFactory;
        private readonly IRawDataExecutor _executor;
        private readonly IBucketingService _bucket; 
        private readonly ILogger<ChartDataService> _log; 

        public ChartDataService(
            IChartQueryPlanner planner,
            ITimeColumnInspector timeInspector,
            ISqlRequestFactory sqlFactory,
            IRawDataExecutor executor,
            IBucketingService bucket,
            ILogger<ChartDataService> log)
        {
            _planner = planner;
            _timeInspector = timeInspector;
            _sqlFactory = sqlFactory;
            _executor = executor;
            _bucket = bucket; 
            _log = log;
        }

    
        public async Task<MultiSeriesResponse> GetMultiSeriesAsync(DbConnection con, GetMultiSeriesRequest request, CancellationToken ct)
        {


            (bool Success, ChartQueryPlan? Plan) = await _planner.BuildPlanAsync(con, request.Template, request.Px, maxPoints: null, bucketMs: request.BucketMs, ct);

            if (!Success) {
                return new MultiSeriesResponse
                {
                    Entity = request.Template.Entity,
                    TimeField = request.Template.TimeField,
                    Series = []
                };
            }


            var timeKind = await _timeInspector.GetKindAsync(con, Plan.Entity, Plan.TimeField, ct);

            var items = new List<MultiSeriesItemDto>(Plan.Fields.Length);

  
            foreach (var field in Plan.Fields)
            {
                var sqlReq = _sqlFactory.BuildRawPoints(Plan.Entity, field.Name, Plan.TimeField, timeKind,
                                                        Plan.From, Plan.To, Plan.Where, Plan.Sql, Plan.ParamCatalog,
                                                        limit: null);
                List<RawPointDto> points = await _executor.ExecutePointsAsync(con, sqlReq, ct); 

                var ms = Plan.BucketMilliseconds;

                 
                if (ms <= 0)
                {                                                
                    ms = _planner.PickBucketMilliseconds(Plan.From, Plan.To, request.Px);
                }

                (List<SeriesBinDto> bins, long alignedFromMs, long alignedToMs) = _bucket.BuildBuckets(Plan.From, Plan.To, Plan.BucketMilliseconds <= 0 ? ms : Plan.BucketMilliseconds, points);

                _log.LogInformation($"Кол-во точек для {field.Name}: {bins.Count}");

                items.Add(new MultiSeriesItemDto { 
                    FromMs = Plan.From,
                    ToMs = Plan.To,
                    AlignedFromMs = alignedFromMs,
                    AlignedToMs = alignedToMs,
                    Field = field,
                    BucketMs = ms,
                    Bins = bins,
                });
            }

          

            return new MultiSeriesResponse
            {
                Entity = request.Template.Entity,
                TimeField = request.Template.TimeField, 
                Series = items
            };
        }
         
 
    }
}
