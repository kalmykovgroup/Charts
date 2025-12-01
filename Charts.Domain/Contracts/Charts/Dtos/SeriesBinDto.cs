namespace Charts.Domain.Contracts.Charts.Dtos
{
    public class SeriesBinDto
    {
        public long T { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        public double? Avg { get; set; }
        public long Count { get; set; } 
    }
}
