using AutoMapper;
using Charts.Api.Application.Contracts;
using Charts.Api.Application.Contracts.Metadata.Dtos;
using Charts.Api.Application.Interfaces;
using Charts.Api.Application.Interfaces.Repositories;
using Charts.Api.Application.QueryAndCommands.Metadata.Databases;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Charts.Api.Application.Handlers.Metadata.Databases
{
    public class UpdateDatabaseHandler(
        IDatabaseRepository repo,
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<UpdateDatabaseHandler> logger
    ) : IRequestHandler<UpdateDatabaseCommand, ApiResponse<DatabaseDto>>
    {
        public async Task<ApiResponse<DatabaseDto>> Handle(UpdateDatabaseCommand command, CancellationToken ct)
        {
            await using var tx = await uow.BeginTransactionAsync(ct);
            try
            {
                var entity = await repo.GetByIdAsync(command.Id, ct);
                if (entity is null)
                {
                    logger.LogWarning("Database not found: {Id}", command.Id);
                    throw new KeyNotFoundException($"Database {command.Id} not found");
                }

                mapper.Map(command.Request, entity); // Id игнорируем в профиле
                await repo.UpdateAsync(entity, ct);
                await uow.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                return ApiResponse<DatabaseDto>.Ok(mapper.Map<DatabaseDto>(entity));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while updating Database Id={Id}", command.Id);
                await tx.RollbackAsync(ct);
                throw;
            }
        }
    }
}
