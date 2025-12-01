using AutoMapper;
using Charts.Application.QueryAndCommands.Template;
using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Template.Dtos;
using Charts.Domain.Interfaces.Repositories;
using MediatR;

namespace Charts.Application.Handlers.ChartReqTemplates
{
    public class GetAllChartReqTemplatesHandler(
        IChartReqTemplateRepository repo,
        IMapper mapper
    ) : IRequestHandler<GetAllChartReqTemplatesQuery, ApiResponse<List<ChartReqTemplateDto>>>
    {
        public async Task<ApiResponse<List<ChartReqTemplateDto>>> Handle(GetAllChartReqTemplatesQuery request, CancellationToken ct)
        {
            var items = await repo.GetAllAsync(ct);
            var dtos = mapper.Map<List<ChartReqTemplateDto>>(items);
            return ApiResponse<List<ChartReqTemplateDto>>.Ok(dtos);
        }
    }

}
