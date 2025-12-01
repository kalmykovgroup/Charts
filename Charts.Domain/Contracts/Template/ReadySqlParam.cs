using Charts.Domain.Contracts.Metadata.Dtos;

namespace Charts.Domain.Contracts.Template
{ 
    public sealed record ReadySqlParam(
    string Key,                      // имя плейсхолдера (например, {{key}})
    object Value,            // явное значение (может быть null, можно не задавать)
    FieldDto Field,          // имя колонки для вывода типа (подсмотреть метаданные) 
    bool Required = false           // если true — при исполнении должно прийти значение (или будет ошибка)
);
}
