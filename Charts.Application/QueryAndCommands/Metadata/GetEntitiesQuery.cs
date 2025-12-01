using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Metadata.Dtos;
using MediatR;

namespace Charts.Application.QueryAndCommands.Metadata
{
    public sealed record GetEntitiesQuery() : IRequest<ApiResponse<IReadOnlyList<EntityDto>>>;
}
