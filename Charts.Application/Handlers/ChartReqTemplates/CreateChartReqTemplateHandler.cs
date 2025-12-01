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

                // BUGFIX: раньше было entity.Id = command.Request.Id != null ? entity.Id : Guid.CreateVersion7();
                entity.Id = command.Request.Id ?? Guid.CreateVersion7();

                await repo.AddAsync(entity, ct);
                await uow.SaveChangesAsync(ct);

                await tx.CommitAsync(ct);           // <-- только DB-операции внутри try
            }
            catch (Exception ex)
            {
                // Откатываем ТОЛЬКО если коммита не было
                try { await tx.RollbackAsync(ct); } catch { /* ignore */ }
                logger.LogError(ex, "Error while creating ChartReqTemplate");
                return ApiResponse<ChartReqTemplateDto>.Fail(ex.Message, ex);
            }

            // Всё, что может упасть, делаем уже вне зоны rollback
            var dto = mapper.Map<ChartReqTemplateDto>(entity);
            return ApiResponse<ChartReqTemplateDto>.Ok(dto);
        }

    }
}
