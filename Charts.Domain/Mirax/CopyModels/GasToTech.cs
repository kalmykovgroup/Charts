using System.ComponentModel.DataAnnotations;

namespace Charts.Api.Domain.Mirax.CopyModels
{
    public class GasToTech
    {
        [Key]
        public Guid Id { get; set; }

        public Gas? GasToUse { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
