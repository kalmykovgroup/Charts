namespace Charts.Api.Infrastructure.Options
{
    /// <summary>
    /// Серверная конфигурация выбора bucket'а (в миллисекундах).
    /// Только фиксированная сетка, БЕЗ календарных шагов.
    /// </summary>
    public sealed class ChartBucketingOptions
    {
        /// <summary>
        /// Сколько целевых точек (бинов) хотим на графике: TargetPoints = max(MinTargetPoints, Px * TargetPointsPerPx).
        /// По умолчанию px/4 как раньше.
        /// </summary>
        public double TargetPointsPerPx { get; set; }  

        /// <summary> Нижняя граница количества бинов. </summary>
        public int MinTargetPoints { get; set; }

        /// <summary>
        /// Фиксированная "красивая" сетка шагов в миллисекундах.
        /// Должна совпадать с клиентом (BUCKETS_MS): 1s, 2s, 5s ...
        /// </summary>
        public long[] NiceMilliseconds { get; set; } = [];

        /// <summary>
        /// Разрешать ли использовать КРАТНЫЕ недели, если rough больше последнего элемента NiceMilliseconds.
        /// Если false — всегда выбирать последний из NiceMilliseconds.
        /// Если true — подбирать N * 7d (в миллисекундах) до MaxWeeksMultiple.
        /// </summary>
        public bool EnableWeeklyMultiples { get; set; } = false;

        /// <summary> Максимальный множитель недель, если EnableWeeklyMultiples=true. </summary>
        public int MaxWeeksMultiple { get; set; }  
    }
}