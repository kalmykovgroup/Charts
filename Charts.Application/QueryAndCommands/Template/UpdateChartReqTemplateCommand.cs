using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Template.Dtos;
using Charts.Domain.Contracts.Template.Requests;
using MediatR;

namespace Charts.Application.QueryAndCommands.Template
{
    public record UpdateChartReqTemplateCommand(Guid Id, UpdateChartReqTemplateRequest Request) : IRequest<ApiResponse<ChartReqTemplateDto>>;

}
