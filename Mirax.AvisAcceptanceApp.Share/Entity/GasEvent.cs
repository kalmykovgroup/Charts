using Mirax.AvisAcceptanceApp.Share.Types;
using System.ComponentModel.DataAnnotations;

namespace Mirax.AvisAcceptanceApp.Service.TechTest.Models
{
    public class GasEvent
    {
        [Key]
        public Guid Id { get; set; }
        public required string GasName { get; set; }
        public required DateTime TimeToStart { get; set; }
        public GasEventType? EventType { get; set; }

    }

}
