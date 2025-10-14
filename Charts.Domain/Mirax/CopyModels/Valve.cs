using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Charts.Api.Domain.Mirax.CopyModels
{
    public class Valve
    {
        [Key]
        public Guid Id { get; set; }
        public required ushort Address { get; set; }
        public required byte SlaveId { get; set; }
        public required bool Value { get; set; }

        [NotMapped]
        public string? Name { get; set; }

        public Gas? Gase { get; set; }

        [ConcurrencyCheck]
        public Guid Version { get; set; }

        [NotMapped]
        public bool IsOpen { get; set; }

        [NotMapped]
        public int Position { get; set; }

        [NotMapped]
        public int OvenSlave { get; set; }
    }
}
