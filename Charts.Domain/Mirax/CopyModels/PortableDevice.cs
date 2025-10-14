using Mirax.AvisAcceptanceApp.Models.Entity.ModBusDevice.PortableDeviceModel;
using Mirax.AvisAcceptanceApp.Share.Types;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace Charts.Api.Domain.Mirax.CopyModels
{
    public partial class PortableDevice : DeviceEntity 
    {
        public string? Name { get; set; }
        public byte? ModbusAddress { get; set; }
        public ClimateTestToStart? ClimateTestToStart { get; set; }

    
        public double BatteryLevel { get; set; }


        public double Temperature { get; set; }


        public bool CriticalBattery { get; set; }


        public double BatteryVoltage { get; set; }
        public bool CriticalLowBattery { get; set; }

        [NotMapped]
        public byte SlaveAdress { get; set; }

        public string? ComPortName { get; set; }
        public int CountOfChanel { get; set; }
        public DeviceTypeEnum DeviceType { get; set; }
        public ObservableCollection<Sensor>? Sensors { get; set; } = new();

        public int ErrorFlag { get; set; } = 0; 

        public DeviceSystemError? DeviceSystemError { get; set; }

        public DeviceSystemStatus? DeviceSystemStatus { get; set; }

  
       
      
 

    }
}
