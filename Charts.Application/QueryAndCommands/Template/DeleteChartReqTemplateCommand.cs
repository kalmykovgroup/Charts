using Charts.Api.Application.Contracts;
using MediatR;

namespace Charts.Api.Application.QueryAndCommands.Template
{
    public record DeleteChartReqTemplateCommand(Guid Id) : IRequest<ApiResponse<bool>>;
}
