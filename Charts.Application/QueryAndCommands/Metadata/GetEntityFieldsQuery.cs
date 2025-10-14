using Charts.Api.Application.Contracts;
using Charts.Api.Application.Contracts.Metadata.Dtos;
using MediatR;

namespace Charts.Api.Application.QueryAndCommands.Metadata
{
    // Получить поля конкретной сущности из базы
    public sealed record GetEntityFieldsQuery(string Entity)
        : IRequest<ApiResponse<IReadOnlyList<FieldDto>>>;
}
