using AutoMapper;
using Charts.Application.QueryAndCommands.Metadata.Databases;
using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Metadata.Dtos;
using Charts.Domain.Contracts.Types;
using Charts.Domain.Interfaces;
using Charts.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Charts.Application.Handlers.Metadata.Databases
{
    public class UpdateDatabaseHandler(
        IDatabaseRepository repo,
        IUnitOfWork uow,
        IMapper mapper,
        IDatabaseRegistry registry,
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

                var wasActive = entity.DatabaseStatus == DatabaseStatus.Active;

                mapper.Map(command.Request, entity);
                await repo.UpdateAsync(entity, ct);
                await uow.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                // Перерегистрируем в реестре
                if (entity.DatabaseStatus == DatabaseStatus.Active)
                {
                    // Удаляем старую регистрацию и добавляем новую (connection string мог измениться)
                    await registry.UnregisterAsync(entity.Id, ct);
                    await registry.RegisterAsync(entity.Id, ct);
                }
                else if (wasActive)
                {
                    // Была активна, стала неактивной — удаляем из реестра
                    await registry.UnregisterAsync(entity.Id, ct);
                }

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
