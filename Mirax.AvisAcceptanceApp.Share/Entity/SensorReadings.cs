using System.ComponentModel.DataAnnotations;

namespace Mirax.AvisAcceptanceApp.Models.DTO.ClimateTest
{
    public class SensorReadings
    {
        [Key]
        public Guid Id { get; set; }
        public int TempPointIndex { get; set; }
        public double NullPoint { get; set; }
        public double ConcentrationPoint { get; set; }
        public double DeviceTemperature { get; set; }
    }
}
