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
    public class UpdateChartReqTemplateHandler(
        IChartReqTemplateRepository repo,
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<UpdateChartReqTemplateHandler> logger
    ) : IRequestHandler<UpdateChartReqTemplateCommand, ApiResponse<ChartReqTemplateDto>>
    {
        public async Task<ApiResponse<ChartReqTemplateDto>> Handle(UpdateChartReqTemplateCommand command, CancellationToken ct)
        {
            ChartReqTemplate? entity;

            await using var tx = await uow.BeginTransactionAsync(ct);
            try
            {
                entity = await repo.GetByIdAsync(command.Id, ct);
                if (entity is null)
                {
                    logger.LogWarning("ChartReqTemplate not found: {Id}", command.Id);
                    throw new KeyNotFoundException($"ChartReqTemplate {command.Id} not found");
                }

                mapper.Map(command.Request, entity);
                await repo.UpdateAsync(entity, ct);
                await uow.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while updating ChartReqTemplate Id={Id}", command.Id);
                await tx.RollbackAsync(ct);
                throw;
            }

            var dto = mapper.Map<ChartReqTemplateDto>(entity);
            return ApiResponse<ChartReqTemplateDto>.Ok(dto);
        }
    }
}
