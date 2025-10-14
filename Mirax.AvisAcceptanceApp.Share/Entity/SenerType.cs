using System.ComponentModel.DataAnnotations;

namespace Mirax.AvisAcceptanceApp.Models.Entity
{
    public class SenerType
    {
        [Key]
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
