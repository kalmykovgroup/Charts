using Mirax.AvisAcceptanceApp.Service.TemperatureAnalysis.Models;
using Mirax.AvisAcceptanceApp.Share.Types;
using System.ComponentModel.DataAnnotations;

namespace Charts.Api.Domain.Mirax.CopyModels
{
    public class TemperatureTestToRun
    {
        [Key]
        public Guid Id { get; set; }

        public string? Name { get; set; }
        public DateTime DateStarTime { get; set; }
        public DateTime DateEndTime { get; set; }
        public List<DeviceContainer>? DeviceContainers { get; set; } = [];
        public List<TempPoint>? TempPoints { get; set; } = [];
        public TemperatureRunStatusEnum TechRunStatus { get; set; }
        public double Progress { get; set; }

        [ConcurrencyCheck]
        public Guid Version { get; set; }

      

    }
}
