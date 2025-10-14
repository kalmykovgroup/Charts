using Mirax.AvisAcceptanceApp.Models.DTO.ClimateTest;
using Mirax.AvisAcceptanceApp.Models.Entity.ModBusDevice.PortableDeviceModel;

namespace Charts.Api.Domain.Mirax.CopyModels
{
    public partial class Sensor : DeviceEntity 
    {

        public string SensorType { get; set; }

        public string SensorName { get; set; }

        public double Span { get; set; }

        public string DisplayUnits { get; set; }

        public string MainUnits { get; set; }

        public int NullAdc { get; set; }

        public string Gas { get; set; }

        public double Concentration { get; set; }

        public int CurrentAdc { get; set; }

        public int SpanAdc { get; set; }

        public double RawConcentration { get; set; }

        public int ChannelNumber { get; set; }

        public double Humidity { get; set; }

        public float CalibrationConcentration { get; set; }
        public string? Modification { get; set; }

        

        public Error? Error { get; set; }
        public ChanelState ChanelState { get; set; } = new();

        public PortableDevice? PortableDevices { get; set; } = new();

        public SensorReadings? ClimateTestSensorReadings { get; set; }
 
 
    }
}
