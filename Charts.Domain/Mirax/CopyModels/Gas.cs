using Mirax.AvisAcceptanceApp.Models.Entity;
using Mirax.AvisAcceptanceApp.Share.Types;
using System.ComponentModel.DataAnnotations;

namespace Charts.Api.Domain.Mirax.CopyModels
{
    public class Gas  
    {
        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string ChemicalFormula { get; set; }

        public required double Concentration { get; set; }

        public required string Units { get; set; }

        public string? ResidualGas { get; set; }

        public string? GasCylinderNumber { get; set; }

        public required DateTime ExpireDate { get; set; }

        public required string StateStandartSampleNumber { get; set; }

        public string? CASNumber { get; set; }

        public List<Valve>? Valves { get; set; }

        public List<ClimateTestGas>? ClimateTestGases { get; set; }

        public List<CreatedClimateTest>? CreatedClimateTest { get; set; }

        public GasStandartSampleType? GasStandartSampleType { get; set; }

        [ConcurrencyCheck]
        public Guid Version { get; set; }
 
    }
}
