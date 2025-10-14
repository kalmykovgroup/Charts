using Charts.Api.Application.Contracts.Charts.Dtos;
using Charts.Api.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace Charts.Api.Infrastructure.Services
{
    /// <summary>
    /// Агрегатор точек во временные ведра.
    /// Важно: игнорируем Kind/TZ — считаем по тикам как есть (интервал [from..to)).
    /// Возвращает список ведер с T=начало ведра, Min/Max/Avg по V.
    /// </summary>
    public sealed class BucketingService : IBucketingService
    {
        private readonly ILogger<BucketingService> _log;
        public BucketingService(ILogger<BucketingService> log) => _log = log;

        public (List<SeriesBinDto> bins, long alignedFromMs, long alignedToMs) BuildBuckets(
            long fromMs,              // Unix timestamp в миллисекундах
            long toMs,                // Unix timestamp в миллисекундах
            long bucketMilliseconds,
            IReadOnlyList<RawPointDto> points)
        {
            // Округляем границы ДО построения bucket'ов
            long alignedFromMs = (fromMs / bucketMilliseconds) * bucketMilliseconds;
            long alignedToMs = ((toMs + bucketMilliseconds - 1) / bucketMilliseconds) * bucketMilliseconds;

            // Конвертируем ОКРУГЛЕННЫЕ границы в тики
            long startTicks = DateTimeOffset.FromUnixTimeMilliseconds(alignedFromMs).Ticks;
            long endTicks = DateTimeOffset.FromUnixTimeMilliseconds(alignedToMs).Ticks;
            long stepTicks = bucketMilliseconds * TimeSpan.TicksPerMillisecond;

            if (stepTicks <= 0)
                throw new ArgumentOutOfRangeException(nameof(bucketMilliseconds));
            if (endTicks <= startTicks)
                return (new List<SeriesBinDto>(), alignedFromMs, alignedToMs);

            // Кол-во ведер для полуинтервала [from..to)
            long spanTicks = endTicks - startTicks;
            int buckets = (int)((spanTicks + stepTicks - 1) / stepTicks);
            if (buckets <= 0) return (new List<SeriesBinDto>(), alignedFromMs, alignedToMs);

            var bins = new List<SeriesBinDto>(buckets);
            // ВАЖНО: создаем вспомогательный массив для накопления суммы
            var sums = new double[buckets];

            for (int i = 0; i < buckets; i++)
            {
                long binTicks = startTicks + i * stepTicks;
                var t = new DateTimeOffset(new DateTime(binTicks, DateTimeKind.Utc)).ToUnixTimeMilliseconds();
                bins.Add(new SeriesBinDto { T = t, Min = null, Max = null, Avg = null, Count = 0 });
            }

            // --- Раскладываем точки по ведрам (строго по тикам) ---
            int outNeg = 0, outPos = 0, nan = 0;
            var span = CollectionsMarshal.AsSpan(bins);

            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                if (!p.Value.HasValue) { nan++; continue; }

                // Конвертируем Unix timestamp точки в тики и вычисляем смещение
                long pointTicks = DateTimeOffset.FromUnixTimeMilliseconds(p.Time).Ticks;
                long dtTicks = pointTicks - startTicks;
                int idx = (int)(dtTicks / stepTicks);

                if (idx < 0) { outNeg++; continue; }
                if (idx >= buckets) { outPos++; continue; }

                ref var b = ref span[idx];
                var val = p.Value.Value;

                if (b.Count == 0)
                {
                    b.Min = b.Max = val;
                    sums[idx] = val;  // начинаем накопление суммы
                    b.Count = 1;
                }
                else
                {
                    if (val < b.Min) b.Min = val;
                    if (val > b.Max) b.Max = val;
                    sums[idx] += val;  // накапливаем сумму
                    b.Count++;
                }
            }

            // ВАЖНО: вычисляем среднее с учетом особых случаев
            for (int i = 0; i < bins.Count; i++)
            {
                if (bins[i].Count > 0)
                {
                    // Если все значения одинаковые (Min == Max), то Avg = это значение
                    // Это устраняет погрешность floating-point при суммировании одинаковых чисел
                    if (bins[i].Min == bins[i].Max)
                    {
                        bins[i].Avg = bins[i].Min;
                    }
                    else
                    {
                        bins[i].Avg = sums[i] / bins[i].Count;
                    }
                }
            }

            _log.LogInformation("BUCKET SUMMARY: in={In}, outNeg={OutNeg}, outPos={OutPos}, nan={NaN}, bins={Bins}",
                points.Count - outNeg - outPos - nan, outNeg, outPos, nan, bins.Count);

            return (bins, alignedFromMs, alignedToMs);
        }
    }
}