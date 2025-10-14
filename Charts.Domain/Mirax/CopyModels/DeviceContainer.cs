using System.ComponentModel.DataAnnotations;

namespace Charts.Api.Domain.Mirax.CopyModels
{
    public partial class DeviceContainer  
    {
        [Key]
        public Guid Id { get; set; }

        public required string ComName { get; set; }
    }
}
