using Charts.Api.Application.Contracts.Metadata.Dtos;

namespace Charts.Api.Domain.Contracts.Template.Dtos
{
    public class ResolvedCharReqTemplate
    {
        public Guid Id { get; set; } 

        public Guid DatabaseId { get; set; } // где исполнять 

        public long? ResolvedFromMs { get; init; }
        public long? ResolvedToMs { get; init; }

        // настройки графиков 
        public EntityDto Entity { get; set; } = null!;
        public FieldDto TimeField { get; set; } = null!;
        public FieldDto[] SelectedFields { get; set; } = [];

        public List<FilterClause>? Where { get; set; } = [];     // допускает {{key}} в value/values
        public SqlFilter? Sql { get; set; }                // Вариант B: только WhereSql + Trusted
        public List<ReadySqlParam>? Params { get; set; } = [];       // ЕДИНЫЙ каталог параметров (для Where и Sql) 
    }
}
