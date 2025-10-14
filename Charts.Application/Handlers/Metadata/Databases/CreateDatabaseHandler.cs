using AutoMapper;
using Charts.Api.Application.Contracts;
using Charts.Api.Application.Contracts.Metadata.Dtos;
using Charts.Api.Application.Interfaces;
using Charts.Api.Application.Interfaces.Repositories;
using Charts.Api.Application.Models;
using Charts.Api.Application.QueryAndCommands.Metadata.Databases;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Charts.Api.Application.Handlers.Metadata.Databases
{
    public class CreateDatabaseHandler(
        IDatabaseRepository repo,
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<CreateDatabaseHandler> logger
    ) : IRequestHandler<CreateDatabaseCommand, ApiResponse<DatabaseDto>>
    {
        public async Task<ApiResponse<DatabaseDto>> Handle(CreateDatabaseCommand command, CancellationToken ct)
        {
            await using var tx = await uow.BeginTransactionAsync(ct);
            try
            {
                var entity = mapper.Map<Database>(command.Request);
                entity.Id = Guid.CreateVersion7(); // генерируем Id (как в твоём примере)

                await repo.AddAsync(entity, ct);
                await uow.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                return ApiResponse<DatabaseDto>.Ok(mapper.Map<DatabaseDto>(entity));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while creating Database");
                await tx.RollbackAsync(ct);
                throw;
            }
        }
    }
}
