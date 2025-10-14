using Charts.Api.Application.Contracts;
using Charts.Api.Application.Contracts.Charts.Responces;
using Charts.Api.Application.Interfaces;
using Charts.Api.Application.QueryAndCommands.Chart;
using MediatR;

namespace Charts.Api.Application.Handlers.Charts
{
    public sealed class GetMultiSeriesHandler(ICurrentDb db, IChartDataService charts) : IRequestHandler<GetMultiSeriesQuery, ApiResponse<MultiSeriesResponse>>
    {
        public async Task<ApiResponse<MultiSeriesResponse>> Handle(GetMultiSeriesQuery req, CancellationToken ct)
        {
            try
            {
                await using var con = await db.OpenConnectionAsync(ct); 

                var dto = await charts.GetMultiSeriesAsync(con, req.Request, ct);
                 
                return ApiResponse<MultiSeriesResponse>.Ok(dto);
            }
            catch (Exception ex)
            {
                return ApiResponse<MultiSeriesResponse>.Fail(ex.Message, ex);
            }           
        } 
    }
}
