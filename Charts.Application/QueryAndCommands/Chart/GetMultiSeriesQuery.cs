using Charts.Api.Application.Contracts;
using Charts.Api.Application.Contracts.Charts.Requests;
using Charts.Api.Application.Contracts.Charts.Responces;
using MediatR;

namespace Charts.Api.Application.QueryAndCommands.Chart
{
    public sealed record GetMultiSeriesQuery(GetMultiSeriesRequest Request) : IRequest<ApiResponse<MultiSeriesResponse>>;
}
