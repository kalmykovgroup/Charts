using AutoMapper;
using Charts.Application.QueryAndCommands.Metadata.Databases;
using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Metadata.Dtos;
using Charts.Domain.Interfaces.Repositories;
using MediatR;

namespace Charts.Application.Handlers.Metadata.Databases;

public sealed class GetAllDatabasesHandler(
    IDatabaseRepository repo,
    IMapper mapper
) : IRequestHandler<GetAllDatabasesQuery, ApiResponse<List<DatabaseDto>>>
{
    public async Task<ApiResponse<List<DatabaseDto>>> Handle(GetAllDatabasesQuery request, CancellationToken ct)
    {
        var items = await repo.GetAllAsync(ct);
        var databases = mapper.Map<List<DatabaseDto>>(items);
        return ApiResponse<List<DatabaseDto>>.Ok(databases);
    }
}
