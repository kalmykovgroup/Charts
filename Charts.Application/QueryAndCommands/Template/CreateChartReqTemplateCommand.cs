using Charts.Api.Application.Contracts;
using Charts.Api.Application.Contracts.Template.Dtos;
using Charts.Api.Application.Contracts.Template.Requests;
using MediatR;

namespace Charts.Api.Application.QueryAndCommands.Template
{
    public record CreateChartReqTemplateCommand(CreateChartReqTemplateRequest Request) : IRequest<ApiResponse<ChartReqTemplateDto>>;

}
