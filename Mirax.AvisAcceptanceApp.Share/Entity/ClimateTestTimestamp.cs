using System.ComponentModel.DataAnnotations;

namespace Mirax.AvisAcceptanceApp.Models.Entity
{
    public class ClimateTestTimestamp
    {
        [Key]
        public Guid Id { get; set; }
        public required DateTime ExperimentStarts { get; set; }
        public required DateTime ExperimentEnds { get; set; }
    }
}
