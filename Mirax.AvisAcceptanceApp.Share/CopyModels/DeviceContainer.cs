using System.ComponentModel.DataAnnotations;

namespace Mirax.AvisAcceptanceApp.Share.CopyModels
{
    public partial class DeviceContainer  
    {
        [Key]
        public Guid Id { get; set; }

        public required string ComName { get; set; }
    }
}
