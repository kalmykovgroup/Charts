using AutoMapper;
using Charts.Api.Application.Contracts;
using Charts.Api.Application.Contracts.Template.Dtos;
using Charts.Api.Application.Interfaces.Repositories;
using Charts.Api.Application.QueryAndCommands.Template;
using MediatR;

namespace Charts.Api.Application.Handlers.ChartReqTemplates
{
    public class GetAllChartReqTemplatesHandler(
        IChartReqTemplateRepository repo,
        IMapper mapper
    ) : IRequestHandler<GetAllChartReqTemplatesQuery, ApiResponse<List<ChartReqTemplateDto>>>
    {
        public async Task<ApiResponse<List<ChartReqTemplateDto>>> Handle(GetAllChartReqTemplatesQuery request, CancellationToken ct)
        {
            try
            {
                var items = await repo.GetAllAsync(ct);
                var dtos = mapper.Map<List<ChartReqTemplateDto>>(items);

                return ApiResponse<List<ChartReqTemplateDto>>.Ok(dtos);
            }
            catch (Exception ex) {
                return ApiResponse<List<ChartReqTemplateDto>>.Fail(ex.Message, ex);
            }
            
        }
    }

}
