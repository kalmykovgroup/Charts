using Mirax.AvisAcceptanceApp.Share.Types;
using System.ComponentModel.DataAnnotations;

namespace Charts.Api.Domain.Mirax.CopyModels
{
    public class ClimateStepLog
    {
        [Key]
        public Guid ID { get; set; }
        public int StepNumber { get; set; }
        public ClimateRunStep ClimateRunStep { get; set; }
        public int CurrentTemperatureInOrder { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid? GasToClimateId { get; set; }
    }
}
