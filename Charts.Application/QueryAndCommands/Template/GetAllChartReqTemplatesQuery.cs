using Charts.Api.Application.Contracts;
using Charts.Api.Application.Contracts.Template.Dtos;
using MediatR;

namespace Charts.Api.Application.QueryAndCommands.Template
{
    public record GetAllChartReqTemplatesQuery() : IRequest<ApiResponse<List<ChartReqTemplateDto>>>;
}
