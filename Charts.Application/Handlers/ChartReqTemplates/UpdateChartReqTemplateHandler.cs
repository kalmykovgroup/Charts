using AutoMapper;
using Charts.Api.Application.Contracts;
using Charts.Api.Application.Contracts.Template.Dtos;
using Charts.Api.Application.Interfaces;
using Charts.Api.Application.Interfaces.Repositories;
using Charts.Api.Application.Models;
using Charts.Api.Application.QueryAndCommands.Template;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Charts.Api.Application.Handlers.ChartReqTemplates
{
    public class UpdateChartReqTemplateHandler(
        IChartReqTemplateRepository repo,
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<UpdateChartReqTemplateHandler> logger
    ) : IRequestHandler<UpdateChartReqTemplateCommand, ApiResponse<ChartReqTemplateDto>>
    {
        public async Task<ApiResponse<ChartReqTemplateDto>> Handle(UpdateChartReqTemplateCommand command, CancellationToken ct)
        {
            ChartReqTemplate? entity = null;

            await using var tx = await uow.BeginTransactionAsync(ct);
            try
            {
                entity = await repo.GetByIdAsync(command.Id, ct);
                if (entity is null)
                {
                    logger.LogWarning("ChartReqTemplate not found: {Id}", command.Id);
                    throw new KeyNotFoundException($"ChartReqTemplate {command.Id} not found");
                }

                // Переносим поля из запроса в существующую сущность
                mapper.Map(command.Request, entity);

                await repo.UpdateAsync(entity, ct);
                await uow.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while updating ChartReqTemplate Id={Id}", command.Id);
                await tx.RollbackAsync(ct);

                return ApiResponse<ChartReqTemplateDto>.Fail(ex.Message, ex);
            }

            var dto = mapper.Map<ChartReqTemplateDto>(entity);
            return ApiResponse<ChartReqTemplateDto>.Ok(dto);
        }
    }
}
