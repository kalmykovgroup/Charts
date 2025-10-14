using Mirax.AvisAcceptanceApp.Share.Types;
using System.ComponentModel.DataAnnotations;

namespace Mirax.AvisAcceptanceApp.Share.CopyModels
{
    public class GasToClimate 
    {
        [Key]
        public Guid Id { get; set; }
        public Gas? GasToUse { get; set; }
        public GasSampleType SampleType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
 

    }
}
