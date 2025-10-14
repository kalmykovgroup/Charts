using Charts.Api.Application.Contracts;
using Charts.Api.Domain.Mirax;
using Charts.Api.Domain.Mirax.CopyModels;
using MediatR;

namespace Charts.Api.Application.QueryAndCommands.Mirax
{
    // Получить все испытания
    public sealed record GetTechnicalRunsQuery(string? FactoryNumber) : IRequest<ApiResponse<List<TechnicalRunToStartDto>>>;

}
