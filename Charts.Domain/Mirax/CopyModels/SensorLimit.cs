using Mirax.AvisAcceptanceApp.Share.Types;
using System.ComponentModel.DataAnnotations;

namespace Charts.Api.Domain.Mirax.CopyModels
{
    public class SensorLimit  
    {
        [Key]
        public Guid Id { get; set; }

        public string? SensorType { get; set; }

        public double MinValue { get; set; }

        public double MaxValue { get; set; }

        public LimitType LimitType { get; set; }
 

    }
}
