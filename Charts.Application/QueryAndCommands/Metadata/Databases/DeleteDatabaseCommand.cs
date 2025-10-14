using Charts.Api.Application.Contracts;
using MediatR;

namespace Charts.Api.Application.QueryAndCommands.Metadata.Databases
{
    public record DeleteDatabaseCommand(Guid Id) : IRequest<ApiResponse<bool>>;
}
