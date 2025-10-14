using Charts.Api.Application.Contracts;
using Charts.Api.Application.Interfaces;
using Charts.Api.Application.Interfaces.Repositories;
using Charts.Api.Application.QueryAndCommands.Template;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Charts.Api.Application.Handlers.ChartReqTemplates
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
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while deleting ChartReqTemplate Id={Id}", command.Id);
                await tx.RollbackAsync(ct);
                return ApiResponse<bool>.Fail(ex.Message, ex);
            }


            return ApiResponse<bool>.Ok(true); // контроллер вернёт { ok: true }
        }
    }
}
