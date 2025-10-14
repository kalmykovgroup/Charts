using Charts.Api.Application.Contracts.Charts.Dtos;

namespace Charts.Api.Application.Interfaces
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
