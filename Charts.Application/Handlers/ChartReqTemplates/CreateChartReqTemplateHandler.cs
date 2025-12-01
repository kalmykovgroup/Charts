using AutoMapper;
using Charts.Application.QueryAndCommands.Template;
using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Template.Dtos;
using Charts.Domain.Interfaces;
using Charts.Domain.Interfaces.Repositories;
using Charts.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Charts.Application.Handlers.ChartReqTemplates
{
    public class CreateChartReqTemplateHandler(
        IChartReqTemplateRepository repo,
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<CreateChartReqTemplateHandler> logger
    ) : IRequestHandler<CreateChartReqTemplateCommand, ApiResponse<ChartReqTemplateDto>>
    {
        public async Task<ApiResponse<ChartReqTemplateDto>> Handle(CreateChartReqTemplateCommand command, CancellationToken ct)
        {
            ChartReqTemplate entity;

            await using var tx = await uow.BeginTransactionAsync(ct);
            try
            {
                entity = mapper.Map<ChartReqTemplate>(command.Request);
                entity.Id = command.Request.Id ?? Guid.CreateVersion7();

                await repo.AddAsync(entity, ct);
                await uow.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (Exception ex)
            {
                try { await tx.RollbackAsync(ct); } catch { /* ignore */ }
                logger.LogError(ex, "Error while creating ChartReqTemplate");
                throw;
            }

            var dto = mapper.Map<ChartReqTemplateDto>(entity);
            return ApiResponse<ChartReqTemplateDto>.Ok(dto);
        }

    }
}
