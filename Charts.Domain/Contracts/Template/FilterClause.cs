using Charts.Api.Application.Contracts.Metadata.Dtos;
using Charts.Api.Domain.Contracts.Types;

namespace Charts.Api.Domain.Contracts.Template
{
    /// <summary>Один предикат: поле + оператор + значение (может быть ключем  {{key}}).</summary>
    public sealed record FilterClause(FieldDto Field, FilterOp Op, object Value);
}
