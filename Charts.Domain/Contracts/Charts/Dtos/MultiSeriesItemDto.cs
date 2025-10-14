using Charts.Api.Application.Contracts.Metadata.Dtos;

namespace Charts.Api.Application.Contracts.Charts.Dtos
{
    public class MultiSeriesItemDto
    {
        public FieldDto Field {  get; set; } = null!;
        public long BucketMs {  get; set; }
        public long FromMs { get; set; }
        public long ToMs { get; set; }
        public long AlignedFromMs { get; set; }
        public long AlignedToMs { get; set; } 
        public IReadOnlyList<SeriesBinDto> Bins { get; set; } = [];

    }
}
