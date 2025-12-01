namespace Charts.Infrastructure.Options
{
    public sealed class ChartDataOptions
    {
        public bool UseTimescale { get; set; } = false;     // использовать time_bucket()
        public int MinPx { get; set; } = 200;               // минимальная ширина
        public int MaxPx { get; set; } = 5000;              // максимальная ширина
        public int TargetBinsPerPxPercent { get; set; } = 80; // целимся в ~80% px по числу бинов
        public int MaxBins { get; set; } = 5000;            // верхняя граница бинов
        public int MaxDaysRange { get; set; } = 180;        // ограничение диапазона (защита)
        public int CacheSeconds { get; set; } = 20;         // кэширование ответа (0 = выкл)

        public bool TrustClientEntityNames { get; set; } = false;   // строго использовать присланные имена
        public bool ResolveNamesAgainstCatalog { get; set; } = true;// и/или разрешать по каталогу
        public string DefaultSchema { get; set; } = "public";       // схема по умолчанию, если не прислали

    }
}
