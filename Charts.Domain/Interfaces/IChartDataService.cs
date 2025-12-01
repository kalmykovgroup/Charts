using System.Data.Common;
using Charts.Domain.Contracts.Charts.Requests;
using Charts.Domain.Contracts.Charts.Responces;

namespace Charts.Domain.Interfaces
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
