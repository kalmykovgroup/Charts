using Charts.Api.Application.Contracts;
using Charts.Api.Application.Contracts.Metadata.Dtos;
using Charts.Api.Application.Models;
using MediatR;

namespace Charts.Api.Application.QueryAndCommands.Metadata.Databases
{ 
    public sealed record GetAllDatabasesQuery() : IRequest<ApiResponse<List<DatabaseDto>>>;
}
