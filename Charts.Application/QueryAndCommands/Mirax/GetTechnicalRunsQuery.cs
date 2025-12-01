using Charts.Domain.Contracts;
using Charts.Domain.Mirax;
using MediatR;

namespace Charts.Application.QueryAndCommands.Mirax
{
    // Получить все испытания
    public sealed record GetTechnicalRunsQuery(string? FactoryNumber) : IRequest<ApiResponse<List<TechnicalRunToStartDto>>>;

}
