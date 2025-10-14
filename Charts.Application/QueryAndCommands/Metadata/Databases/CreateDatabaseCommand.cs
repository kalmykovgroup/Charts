using Charts.Api.Application.Contracts;
using Charts.Api.Application.Contracts.Metadata.Dtos;
using Charts.Api.Application.Contracts.Metadata.Requests;
using MediatR;

namespace Charts.Api.Application.QueryAndCommands.Metadata.Databases
{
    public record CreateDatabaseCommand(
        CreateDatabaseRequest Request
    ) : IRequest<ApiResponse<DatabaseDto>>;
}
