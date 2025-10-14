using Charts.Api.Application.Contracts.Charts.Dtos;
using Charts.Api.Application.Contracts.Metadata.Dtos;

namespace Charts.Api.Application.Contracts.Charts.Responces
{
    public class MultiSeriesResponse
    {
        public EntityDto Entity { get; set; } = null!;
        public FieldDto TimeField { get; set; } = null!; 

        public IReadOnlyList<MultiSeriesItemDto> Series { get; set; } = [];
         
        
    }
}
