using Mirax.AvisAcceptanceApp.Share.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mirax.AvisAcceptanceApp.Share.CopyModels
{
    public abstract class DeviceEntity  
    {
        [Key]
        public Guid Id { get; set; }

        public DeviceTypeEnum DeviceType { get; set; }

        public string? FactoryNumber { get; set; }

        public DateTime CreateDate { get; set; }

        public string Discriminator { get; set; }

        public TechnicalRunToStart? TechnicalRunToStart { get; set; }
        //TODO
        [NotMapped]
        public ClimateTestToStart? CreatedClimateTest { get; set; }
    }
}
