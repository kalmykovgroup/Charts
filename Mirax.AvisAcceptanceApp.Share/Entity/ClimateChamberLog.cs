using System.ComponentModel.DataAnnotations;

namespace Mirax.AvisAcceptanceApp.Models.Entity.ModBusDevice
{
    public class ClimateChamberLog
    {
        [Key]
        public Guid Id { get; set; }
        public double TempInChamber { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
