using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Charts.Requests;
using Charts.Domain.Contracts.Charts.Responces;
using MediatR;

namespace Charts.Application.QueryAndCommands.Chart
{
    public sealed record GetMultiSeriesQuery(GetMultiSeriesRequest Request) : IRequest<ApiResponse<MultiSeriesResponse>>;
}
