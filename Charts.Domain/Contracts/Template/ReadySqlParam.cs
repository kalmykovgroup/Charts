using Charts.Api.Application.Contracts.Metadata.Dtos;
using Charts.Api.Domain.Contracts.Types;

namespace Charts.Api.Domain.Contracts.Template
{ 
    public sealed record ReadySqlParam(
    string Key,                      // имя плейсхолдера (например, {{key}})
    object Value,            // явное значение (может быть null, можно не задавать)
    FieldDto Field,          // имя колонки для вывода типа (подсмотреть метаданные) 
    bool Required = false           // если true — при исполнении должно прийти значение (или будет ошибка)
);
}
