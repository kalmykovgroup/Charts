namespace Charts.Api.Domain.Contracts.Template
{

    /// <summary>
    /// WHERE-фрагмент с плейсхолдерами {{key}}. Вариант B: ТОЛЬКО текст.
    /// </summary>
    public sealed record SqlFilter(string WhereSql);


}
