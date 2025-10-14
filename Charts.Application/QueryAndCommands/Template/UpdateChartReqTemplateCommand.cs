using Charts.Api.Application.Contracts;
using Charts.Api.Application.Contracts.Template.Dtos;
using Charts.Api.Application.Contracts.Template.Requests;
using MediatR;

namespace Charts.Api.Application.QueryAndCommands.Template
{
    public record UpdateChartReqTemplateCommand(Guid Id, UpdateChartReqTemplateRequest Request) : IRequest<ApiResponse<ChartReqTemplateDto>>;

}
