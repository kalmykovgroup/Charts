using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Metadata.Dtos;
using Charts.Domain.Contracts.Metadata.Requests;
using MediatR;

namespace Charts.Application.QueryAndCommands.Metadata.Databases
{
    public record UpdateDatabaseCommand(
        Guid Id,
        UpdateDatabaseRequest Request
    ) : IRequest<ApiResponse<DatabaseDto>>;
}
