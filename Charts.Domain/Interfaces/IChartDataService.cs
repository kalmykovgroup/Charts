using Charts.Api.Application.Contracts.Charts.Requests;
using Charts.Api.Application.Contracts.Charts.Responces; 
using System.Data.Common;

namespace Charts.Api.Application.Interfaces
{
    // Контракты, которые уже есть у вас в проекте:
    // - IChartsDbResolver
    // - IEntityMetadataService
    // - DTO/Requests: SeriesBinDto, SeriesResponseDto, MultiSeriesItemDto, MultiSeriesResponseDto,
    //                 GetSeriesRequest, GetMultiSeriesRequest
     public interface IChartDataService
    { 
        Task<MultiSeriesResponse> GetMultiSeriesAsync(DbConnection con, GetMultiSeriesRequest req, CancellationToken ct); 

    }
}
