using Mirax.AvisAcceptanceApp.Share.Types;
using System.ComponentModel.DataAnnotations;

namespace Charts.Api.Domain.Mirax.CopyModels
{
    public class SensorLimitClimate
    {
        [Key]
        public Guid Id { get; set; }
        public string SensorType { get; set; }

        public double MinValue { get; set; }

        public double MaxValue { get; set; }

        public LimitTypeClimate LimitType { get; set; }

        public GasToClimate? GasToClimate { get; set; }
    }
}
