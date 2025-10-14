using System.ComponentModel.DataAnnotations;

namespace Mirax.AvisAcceptanceApp.Models.DTO.ClimateTest
{
    public class ClimatePoint
    {
        [Key]
        public Guid Id { get; set; }
        public int Index { get; set; }
        public double Temperature { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime HoldTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
