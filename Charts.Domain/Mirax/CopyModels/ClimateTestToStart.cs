using Mirax.AvisAcceptanceApp.Models.DTO.ClimateTest;
using Mirax.AvisAcceptanceApp.Models.Entity;
using Mirax.AvisAcceptanceApp.Share.Types;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Charts.Api.Domain.Mirax.CopyModels
{
    public class ClimateTestToStart  
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public ObservableCollection<DeviceContainer>? DeviceContainers { get; set; } = new();

        public ObservableCollection<GasToClimate>? GasToClimates { get; set; } = new();

        public ObservableCollection<SensorLimitClimate>? SensorLimits { get; set; } = new();

        public ObservableCollection<ClimatePoint>? ClimatePoints { get; set; } = new();

        public ClimateGasUseMode GasUseMode { get; set; }

        public DateTime DateStartTime { get; set; }

        public DateTime DateEndTime { get; set; }

        public double MinimumСhargeLevel { get; set; }

        public ClimateTestStatus ClimateTestRunStatus { get; set; }

        public double Progress { get; set; }

        public int GasPurgePeriod { get; set; }

        public int GasSupplyPeriod { get; set; }

        public List<ClimateTestLog>? ClimateTests { get; set; } = new();

        [ConcurrencyCheck]
        public Guid Version { get; set; }

       
    }
}
