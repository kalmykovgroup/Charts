using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Metadata.Dtos;
using Charts.Domain.Contracts.Template;

namespace Charts.Domain.Models
{
    public class ChartReqTemplate : BaseEntity
    { 
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } 

        public Guid DatabaseId { get; set; } // где исполнять
        public Database Database { get; set; } = null!;

        public int VisualOrder { get; set; }

        public long? OriginalFromMs { get; init; }
        public long? OriginalToMs { get; init; }

        // настройки графиков 
        public EntityDto Entity { get; set; } = null!;
        public FieldDto TimeField { get; set; } = null!;
        public FieldDto[] SelectedFields { get; set; } = [];

        public List<FilterClause>? Where { get; set; }     // допускает {{key}} в value/values
        public SqlFilter? Sql { get; set; }                // Вариант B: только WhereSql + Trusted
        public List<SqlParam>? Params { get; set; }        // ЕДИНЫЙ каталог параметров (для Where и Sql)

    }
}
