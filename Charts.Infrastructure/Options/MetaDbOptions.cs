namespace Charts.Api.Infrastructure.Options;

/// <summary>
/// Настройки инициализации схемы мета-БД (AppDbContext).
/// Отвечает ТОЛЬКО за создание таблиц/миграций.
/// Заполнение данными - через SeederPipeline.
/// </summary>
public sealed class MetaDbOptions
{
    public bool AutoMigrate { get; set; } = true;
    public bool RecreateOnStartup { get; set; } = false;
    public bool FailOnConnectionError { get; set; } = true;
    public int RetryAttempts { get; set; } = 5;
    public int RetryDelaySeconds { get; set; } = 2;

    // ДОБАВИТЬ:
    public bool RunSeeders { get; set; } = true;
    public bool FailOnSeederError { get; set; } = false;
}