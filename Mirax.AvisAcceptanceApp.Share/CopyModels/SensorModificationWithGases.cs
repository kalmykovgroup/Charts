using Mirax.AvisAcceptanceApp.Share.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mirax.AvisAcceptanceApp.Share.CopyModels
{
    public class SensorModificationWithGases
    {
        [Key]
        public Guid Id { get; set; }

        public string SensorModification { get; set; }
        public string? Units { get; set; }
        [ForeignKey(nameof(SpanGas))]
        public Guid SpanGasId { get; set; }
        public double ZeroDrift { get; set; }
        public double SpanInaccuracy { get; set; }
        public InaccuracyType InaccuracyType { get; set; }

        [NotMapped]
        public Gas? SpanGas { get; set; }

        [ConcurrencyCheck]
        public Guid Version { get; set; }

    }
}
