using Charts.Application.QueryAndCommands.Metadata.Databases;
using Charts.Domain.Contracts;
using Charts.Domain.Interfaces;
using Charts.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Charts.Application.Handlers.Metadata.Databases
{
    public class DeleteDatabaseHandler(
        IDatabaseRepository repo,
        IUnitOfWork uow,
        IDatabaseRegistry registry,
        ILogger<DeleteDatabaseHandler> logger
    ) : IRequestHandler<DeleteDatabaseCommand, ApiResponse<bool>>
    {
        public async Task<ApiResponse<bool>> Handle(DeleteDatabaseCommand command, CancellationToken ct)
        {
            await using var tx = await uow.BeginTransactionAsync(ct);
            try
            {
                await registry.UnregisterAsync(command.Id, ct);
                await repo.DeleteAsync(command.Id, ct);
                await uow.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return ApiResponse<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while deleting Database Id={Id}", command.Id);
                await tx.RollbackAsync(ct);
                throw;
            }
        }
    }
}
