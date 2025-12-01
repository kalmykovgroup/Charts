using Charts.Domain.Contracts;
using MediatR;

namespace Charts.Application.QueryAndCommands.Template
{
    public record DeleteChartReqTemplateCommand(Guid Id) : IRequest<ApiResponse<bool>>;
}
