using Charts.Api.Application.Contracts;
using Charts.Api.Application.Interfaces;
using Charts.Api.Application.Interfaces.Repositories;
using Charts.Api.Application.QueryAndCommands.Metadata.Databases;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Charts.Api.Application.Handlers.Metadata.Databases
{
    public class DeleteDatabaseHandler(
        IDatabaseRepository repo,
        IUnitOfWork uow,
        ILogger<DeleteDatabaseHandler> logger
    ) : IRequestHandler<DeleteDatabaseCommand, ApiResponse<bool>>
    {
        public async Task<ApiResponse<bool>> Handle(DeleteDatabaseCommand command, CancellationToken ct)
        {
            await using var tx = await uow.BeginTransactionAsync(ct);
            try
            {
                await repo.DeleteAsync(command.Id, ct);
                await uow.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return ApiResponse <bool>.Ok(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while deleting Database Id={Id}", command.Id);
                await tx.RollbackAsync(ct);
                return ApiResponse<bool>.Fail(ex.Message, ex);
            }
        }
    }
}
