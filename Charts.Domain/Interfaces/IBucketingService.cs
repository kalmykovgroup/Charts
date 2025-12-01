using Charts.Domain.Contracts.Charts.Dtos;

namespace Charts.Domain.Interfaces
{
    public interface IBucketingService
    {
        /// <summary>
        /// Строит равномерные ведра по шагу bucketSeconds на диапазоне [fromInclusive .. toExclusive).
        /// Ведро содержит агрегаты по значениям Value точек, попадающих в полуинтервал.
        /// </summary>
        (List<SeriesBinDto> bins, long alignedFromMs, long alignedToMs) BuildBuckets(
            long fromInclusive,
            long toExclusive,
            long bucketSeconds,
            IReadOnlyList<RawPointDto> points);
    }
}
