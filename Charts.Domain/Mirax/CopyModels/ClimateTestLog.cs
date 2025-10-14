using Mirax.AvisAcceptanceApp.Models.Entity.ModBusDevice;
using System.ComponentModel.DataAnnotations;

namespace Charts.Api.Domain.Mirax.CopyModels
{
    public class ClimateTestLog
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime? DateToStart { get; set; }
        public DateTime? DateToEnd { get; set; }
        public List<PortableDevice>? PortableDevices { get; set; } = [];
        public CreatedClimateTest? CreatedClimateTest { get; set; } = new();
        public List<ClimateChamberLog>? ClimateChamberLogs { get; set; } = [];
    }
}
