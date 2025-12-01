namespace Charts.Infrastructure.Options
{
    public sealed class ChartsOptions
    {
        /// <summary>Часовая зона, которой мы будем считать "локальной", когда клиент прислал Unspecified.</summary>
        public string AssumeTimeZone { get; set; } = string.Empty;
    }
}
