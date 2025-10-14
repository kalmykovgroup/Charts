using System.ComponentModel.DataAnnotations;

namespace Charts.Api.Domain.Mirax.CopyModels
{
    public class ClimateGas
    {
        [Key]
        public Guid Id { get; set; }
        public int NumberInOrder { get; set; }
        public Gas Gas { get; set; } = null!;
        public string ConfigurationName { get; set; } = null!;
    }
}
