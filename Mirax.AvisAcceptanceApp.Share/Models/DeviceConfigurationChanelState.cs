using Mirax.AvisAcceptanceApp.Models.Entity.Configuration;
using Mirax.AvisAcceptanceApp.Share.Types;

namespace Mirax.AvisAcceptanceApp.Share.Entity
{
 
    public class DeviceConfigurationChanelState()
    {
        public bool? isActive { get; set; }
        public string? Formula { get; set; }
        public string? SensorName { get; set; }

        /// <summary>
        /// Позиция канала на приборе
        /// </summary>
        public int? ChanelPostion { get; set; }


        #region Тип сенсора

        public SensorType? SensorType { get; set; }

        public UartSensorType? UartSensorType { get; set; }

        public EcSensorSubtype? EcSensorSubtype { get; set; }

        public O2SensorSubtype? O2SensorSubtype { get; set; }

        #endregion
    }
}
