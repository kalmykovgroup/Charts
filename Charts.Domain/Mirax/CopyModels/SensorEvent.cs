using Mirax.AvisAcceptanceApp.Share.Types;
using System.ComponentModel.DataAnnotations;

namespace Charts.Api.Domain.Mirax.CopyModels
{
    public class SensorEvent
    {
        [Key]
        public Guid Id { get; set; }

        public string? SensorModification { get; set; }

        public required DateTime TimeToChange { get; set; }

        public TechnicalRunToStart? TemperatureTestToRun { get; set; }

        public SensorEventType? SensorEventType { get; set; }

    }

}
