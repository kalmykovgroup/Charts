using Charts.Domain.Contracts;
using Charts.Domain.Interfaces;
using MediatR;

namespace Charts.Application.QueryAndCommands.Metadata.Databases
{
    public record TestConnectionCommand(string ConnectionString, string Provider) : IRequest<ApiResponse<ConnectionTestResult>>;
}
