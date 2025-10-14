using System.ComponentModel.DataAnnotations;

namespace Mirax.AvisAcceptanceApp.Models.Entity
{
    public class ClimateTemperature
    {
        [Key]
        public Guid Id { get; set; }
        public int NumberInOrder { get; set; }
        public double Temperature { get; set; }
        public string ConfigurationName { get; set; } = null!;
    }
}
