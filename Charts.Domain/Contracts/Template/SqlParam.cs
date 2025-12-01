using Charts.Domain.Contracts.Metadata.Dtos;

namespace Charts.Domain.Contracts.Template
{
    /// <summary>Описание одного параметра SQL-шаблона.</summary>
    public sealed record SqlParam(
        string Key,                      // имя плейсхолдера (например, {{key}})
        FieldDto Field = null!,          // имя колонки для вывода типа (подсмотреть метаданные) 
        bool Required = false,           // если true — при исполнении должно прийти значение (или будет ошибка)
        object? Value = null,            // явное значение (может быть null, можно не задавать)
        string? Description = null,      // описание для UI/формы
        object? DefaultValue = null
    );
}
