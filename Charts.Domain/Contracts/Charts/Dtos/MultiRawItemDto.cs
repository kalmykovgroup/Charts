using Charts.Api.Application.Contracts.Charts.Dtos;
using Charts.Api.Application.Contracts.Metadata.Dtos;

namespace Charts.Api.Domain.Contracts.Charts.Dtos
{
    public class MultiRawItemDto
    {
        public FieldDto Field { get; set; } = null!;
        public IReadOnlyList<RawPointDto> Points { get; set; } = [];
    }
}
