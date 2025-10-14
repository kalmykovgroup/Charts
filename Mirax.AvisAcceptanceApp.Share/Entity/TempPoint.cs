namespace Mirax.AvisAcceptanceApp.Service.TemperatureAnalysis.Models
{
    public class TempPoint
    {
        public Guid Id { get; set; }

        public TimeSpan TimeToSet { get; set; }

        public TimeSpan TransitionTime { get; set; }

        public double Temperature { get; set; }

        public int Index { get; set; }
    }
}
