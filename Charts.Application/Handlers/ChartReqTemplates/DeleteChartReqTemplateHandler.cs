using Charts.Application.QueryAndCommands.Template;
using Charts.Domain.Contracts;
using Charts.Domain.Interfaces;
using Charts.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Charts.Application.Handlers.ChartReqTemplates
{
    public class DeleteChartReqTemplateHandler(
        IChartReqTemplateRepository repo,
        IUnitOfWork uow,
        ILogger<DeleteChartReqTemplateHandler> logger
    ) : IRequestHandler<DeleteChartReqTemplateCommand, ApiResponse<bool>>
    {
        public async Task<ApiResponse<bool>> Handle(DeleteChartReqTemplateCommand command, CancellationToken ct)
        {
            await using var tx = await uow.BeginTransactionAsync(ct);
            try
            {
                await repo.DeleteAsync(command.Id, ct);
                await uow.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return ApiResponse<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while deleting ChartReqTemplate Id={Id}", command.Id);
                await tx.RollbackAsync(ct);
                throw;
            }
        }
    }
}
