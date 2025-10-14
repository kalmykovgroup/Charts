using Charts.Api.Application.Contracts;
using Charts.Api.Application.Contracts.Metadata.Dtos;
using MediatR;

namespace Charts.Api.Application.QueryAndCommands.Metadata
{
    public sealed record GetEntitiesQuery() : IRequest<ApiResponse<IReadOnlyList<EntityDto>>>;
}
