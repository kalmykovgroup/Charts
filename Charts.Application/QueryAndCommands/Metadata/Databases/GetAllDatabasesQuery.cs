using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Metadata.Dtos;
using MediatR;

namespace Charts.Application.QueryAndCommands.Metadata.Databases
{ 
    public sealed record GetAllDatabasesQuery() : IRequest<ApiResponse<List<DatabaseDto>>>;
}
