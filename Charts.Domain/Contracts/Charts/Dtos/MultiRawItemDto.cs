using Charts.Domain.Contracts.Metadata.Dtos;

namespace Charts.Domain.Contracts.Charts.Dtos
{
    public class MultiRawItemDto
    {
        public FieldDto Field { get; set; } = null!;
        public IReadOnlyList<RawPointDto> Points { get; set; } = [];
    }
}
