using System.ComponentModel.DataAnnotations;

namespace Mirax.AvisAcceptanceApp.Share.CopyModels
{
    public class ClimateTestGas 
    {
        [Key]
        public Guid Id { get; set; }
        public int Position { get; set; }

        public Gas? TestGas { get; set; }

        public TimeSpan Duration { get; set; }
        public bool IsNeedToPurge { get; set; }
        public TimeSpan PurgeDuration { get; set; }

    }
}
