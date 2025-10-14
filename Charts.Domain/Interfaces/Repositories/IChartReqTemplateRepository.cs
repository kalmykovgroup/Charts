using Charts.Api.Application.Interfaces.Repositories.CommonInterfaces;
using Charts.Api.Application.Models;

namespace Charts.Api.Application.Interfaces.Repositories
{ 
    public interface IChartReqTemplateRepository :
    IGetAllRepository<ChartReqTemplate>,
    IAddRepository<ChartReqTemplate>,
    IUpdateRepository<ChartReqTemplate>,
    IDeleteRepository<ChartReqTemplate>,
    IGetByIdRepository<ChartReqTemplate>
    {
    }
}
