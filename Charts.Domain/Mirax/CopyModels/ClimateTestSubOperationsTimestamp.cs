using Mirax.AvisAcceptanceApp.Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace Charts.Api.Domain.Mirax.CopyModels
{
    public class ClimateTestSubOperationsTimestamp
    {
        [Key]
        public Guid Id { get; set; }
        public required ClimateTemperature TemperaturePoint { get; set; }
        public required ClimateGas GasPoint { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
    }
}
