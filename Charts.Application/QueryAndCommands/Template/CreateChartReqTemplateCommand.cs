using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Template.Dtos;
using Charts.Domain.Contracts.Template.Requests;
using MediatR;

namespace Charts.Application.QueryAndCommands.Template
{
    public record CreateChartReqTemplateCommand(CreateChartReqTemplateRequest Request) : IRequest<ApiResponse<ChartReqTemplateDto>>;

}
