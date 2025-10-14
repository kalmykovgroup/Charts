namespace Mirax.AvisAcceptanceApp.Service.TemperatureAnalysis.Models
{
    public class TemperatureData
    {
        public Guid Id { get; set; }

        public DateTime DateTime { get; set; }

        public double TempOnChamber { get; set; }

        public double TempOnSensor { get; set; }

        public string? SensorModification { get; set; }

        public double Consentration { get; set; }

    }
}
