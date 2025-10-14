using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mirax.AvisAcceptanceApp.Models.Entity
{
    public class TypeDefinitions
    {
        [Key]
        public Guid Guid { get; set; }

        [NotMapped]
        public string? Id { get; set; }
        public string? DeviceLine { get; set; }
        public string? Analyte { get; set; }
        public string? Formula { get; set; }
        public string? SensorModification { get; set; }
        public string? LowerLimitOfMeasuringRange { get; set; }
        public string? UpperLimitOfMeasuringRange { get; set; }
        public string? MeasureUnits { get; set; }
        public string? AbsoluteError { get; set; }
        public string? ReducedError { get; set; }
        public string? RelativeError { get; set; }
        public string? IsErrorNeedToCalculate { get; set; }
        public string? AbsoluteErrorFormula { get; set; }
        public string? CASNumber { get; set; }
    }
}
