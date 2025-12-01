using Charts.Domain.Interfaces.Repositories.CommonInterfaces;
using Charts.Domain.Models;

namespace Charts.Domain.Interfaces.Repositories
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
