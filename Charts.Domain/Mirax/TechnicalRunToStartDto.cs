namespace Charts.Domain.Mirax
{
    public class TechnicalRunToStartDto
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public DateTimeOffset DateStartTime { get; set; }

        public DateTimeOffset DateEndTime { get; set; }
 
    }
}
