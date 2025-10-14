using Charts.Api.Application.Contracts.Metadata.Dtos;
using Charts.Api.Domain.Contracts.Template;

namespace Charts.Api.Application.Contracts.Template.Requests
{
    public sealed class UpdateChartReqTemplateRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public Guid DatabaseId { get; set; }  

        //Это исходный при старте графика
        public long? OriginalFromMs { get; init; }
        public long? OriginalToMs { get; init; }

        // настройки графиков 
        public EntityDto Entity { get; set; } = null!;
        public FieldDto TimeField { get; set; } = null!;
        public FieldDto[] SelectedFields { get; set; } = [];

        public List<FilterClause>? Where { get; set; } = [];     // допускает {{key}} в value/values
        public SqlFilter? Sql { get; set; }                // Вариант B: только WhereSql + Trusted
        public List<SqlParam>? Params { get; set; } = [];       // ЕДИНЫЙ каталог параметров (для Where и Sql)  
    }
}
