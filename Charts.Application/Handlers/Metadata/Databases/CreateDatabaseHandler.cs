using AutoMapper;
using Charts.Application.QueryAndCommands.Metadata.Databases;
using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Metadata.Dtos;
using Charts.Domain.Contracts.Types;
using Charts.Domain.Interfaces;
using Charts.Domain.Interfaces.Repositories;
using Charts.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Charts.Application.Handlers.Metadata.Databases
{
    public class CreateDatabaseHandler(
        IDatabaseRepository repo,
        IUnitOfWork uow,
        IMapper mapper,
        IDatabaseRegistry registry,
        ILogger<CreateDatabaseHandler> logger
    ) : IRequestHandler<CreateDatabaseCommand, ApiResponse<DatabaseDto>>
    {
        public async Task<ApiResponse<DatabaseDto>> Handle(CreateDatabaseCommand command, CancellationToken ct)
        {
            await using var tx = await uow.BeginTransactionAsync(ct);
            try
            {
                var entity = mapper.Map<Database>(command.Request);
                entity.Id = Guid.CreateVersion7();

                await repo.AddAsync(entity, ct);
                await uow.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                // Регистрируем в реестре если база активна
                if (entity.DatabaseStatus == DatabaseStatus.Active)
                {
                    await registry.RegisterAsync(entity.Id, ct);
                }

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
