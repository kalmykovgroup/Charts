using Charts.Api.Application.Contracts.Template.Dtos;
using Charts.Api.Domain.Contracts.Template.Dtos;

namespace Charts.Api.Application.Contracts.Charts.Requests
{
    /// <summary>
    /// Несколько серий по шаблону (одна сущность, общие фильтры).
    /// Передаём Template и значения параметров (Values). Можно опционально переопределить список полей.
    /// </summary>
    public sealed class GetMultiSeriesRequest
    {
        public ResolvedCharReqTemplate Template { get; init; } = null!;

        //Это изменяемый fromMs и toMs
        public int Px { get; init; } = 1200; 
        public int? BucketMs { get; init; }  
    }
}
