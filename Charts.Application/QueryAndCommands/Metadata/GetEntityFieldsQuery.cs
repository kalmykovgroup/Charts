using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Metadata.Dtos;
using MediatR;

namespace Charts.Application.QueryAndCommands.Metadata
{
    // Получить поля конкретной сущности из базы
    public sealed record GetEntityFieldsQuery(string Entity)
        : IRequest<ApiResponse<IReadOnlyList<FieldDto>>>;
}
