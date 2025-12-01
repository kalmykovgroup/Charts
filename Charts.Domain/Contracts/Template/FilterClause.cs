using Charts.Domain.Contracts.Metadata.Dtos;
using Charts.Domain.Contracts.Types;

namespace Charts.Domain.Contracts.Template
{
    /// <summary>Один предикат: поле + оператор + значение (может быть ключем  {{key}}).</summary>
    public sealed record FilterClause(FieldDto Field, FilterOp Op, object Value);
}
