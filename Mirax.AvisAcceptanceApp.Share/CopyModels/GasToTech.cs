using System.ComponentModel.DataAnnotations;

namespace Mirax.AvisAcceptanceApp.Share.CopyModels
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
