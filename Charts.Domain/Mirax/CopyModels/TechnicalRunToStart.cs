using Mirax.AvisAcceptanceApp.Service.TechTest.Models;
using Mirax.AvisAcceptanceApp.Share.Types;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Charts.Api.Domain.Mirax.CopyModels
{
    public class TechnicalRunToStart : ICloneable
    {
        [Key]
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public ObservableCollection<DeviceContainer>? DeviceContainers { get; set; } = [];

        public ObservableCollection<GasToTech>? GasToTeches { get; set; } = [];

        public ObservableCollection<SensorLimit>? SensorLimits { get; set; } = [];

        public GasUseModeEnum GasUseMode { get; set; }

        /// <summary>
        /// Ивенты по газам во время испытания
        /// </summary>
        public List<GasEvent>? GasEvents { get; set; }

        /// <summary>
        /// Ивенты по сенсорам во время испытания
        /// </summary>
        public List<SensorEvent>? SensorEvents { get; set; }

        public DateTime DateStarTime { get; set; }

        public DateTime DateEndTime { get; set; }

        public TimeSpan DurationOfTheExperiment { get; set; }

        public double MinimumСhargeLevel { get; set; }

        public int MinimumVoltageLevel { get; set; }

        public int MaximumVoltageLevel { get; set; }

        public TechRunStatusEnum TechRunStatus { get; set; }

        public double Progress { get; set; }

        public int PurgeTime { get; set; }

        public List<TechRunLog>? TechRuns { get; set; } = [];

        [ConcurrencyCheck]
        public Guid Version { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

}
