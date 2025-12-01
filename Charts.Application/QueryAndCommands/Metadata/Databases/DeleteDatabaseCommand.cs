using Charts.Domain.Contracts;
using MediatR;

namespace Charts.Application.QueryAndCommands.Metadata.Databases
{
    public record DeleteDatabaseCommand(Guid Id) : IRequest<ApiResponse<bool>>;
}
