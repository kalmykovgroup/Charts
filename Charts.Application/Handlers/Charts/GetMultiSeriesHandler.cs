using Charts.Application.QueryAndCommands.Chart;
using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Charts.Responces;
using Charts.Domain.Interfaces;
using MediatR;

namespace Charts.Application.Handlers.Charts
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
