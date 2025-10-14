namespace Mirax.AvisAcceptanceApp.Service.TemperatureAnalysis.Models
{
    public class TemperatureAnalysisModel
    {
        public Guid Id { get; set; }

        public required string FactoryNumber { get; set; }

        public List<TemperatureData>? TemperatureDatas { get; set; } = new();

    }
}
