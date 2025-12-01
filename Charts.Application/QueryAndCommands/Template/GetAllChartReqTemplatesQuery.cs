using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Template.Dtos;
using MediatR;

namespace Charts.Application.QueryAndCommands.Template
{
    public record GetAllChartReqTemplatesQuery() : IRequest<ApiResponse<List<ChartReqTemplateDto>>>;
}
