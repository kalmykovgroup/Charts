namespace Charts.Domain.Mirax
{
    public class PortableDeviceDto
    {
      
        public Guid Id { get; set; }
         
        public string FactoryNumber { get; set; } = string.Empty;

        public string? Name { get; set; }
        public string? ComPortName { get; set; }

        public Guid TechnicalRunToStartId { get; set; } 

    }
}
