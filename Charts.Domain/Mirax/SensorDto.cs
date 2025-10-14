namespace Charts.Api.Domain.Mirax
{
    public class SensorDto
    {
        public Guid Id { get; set; }
        public string Gas { get; set; } = string.Empty;
          
        public int ChannelNumber { get; set; }
          
        public string? Modification { get; set; }

        public Guid TechnicalRunToStartId { get; set; }

    }
}
