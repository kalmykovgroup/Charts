using Charts.Domain.Contracts.Charts.Dtos;
using Charts.Domain.Contracts.Metadata.Dtos;

namespace Charts.Domain.Contracts.Charts.Responces
{
    public class MultiSeriesResponse
    {
        public EntityDto Entity { get; set; } = null!;
        public FieldDto TimeField { get; set; } = null!; 

        public IReadOnlyList<MultiSeriesItemDto> Series { get; set; } = [];
         
        
    }
}
