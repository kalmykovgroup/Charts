using System.ComponentModel.DataAnnotations;

namespace Mirax.AvisAcceptanceApp.Share.CopyModels
{
    public class TechRunLog
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime? DataToStart { get; set; }

        public DateTime? DateToEnd { get; set; }

        public List<PortableDevice>? PortableDevice { get; set; } = new();

        public TechnicalRunToStart TechnicalRunToStart { get; set; } = new();
    }
}
