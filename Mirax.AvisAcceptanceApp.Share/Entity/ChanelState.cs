using System.ComponentModel.DataAnnotations;

namespace Mirax.AvisAcceptanceApp.Models.Entity.ModBusDevice.PortableDeviceModel
{
    public class ChanelState
    {
        [Key]
        public Guid Id { get; set; }

        public bool AutoZero { get; set; }

        public bool AutoSpan { get; set; }

        public bool LPTIA { get; set; }

        public bool Bit4 { get; set; }

        public bool WarmSensor { get; set; }

        public bool ChannelTurnedOff { get; set; }

        public bool RangeCalibrationLimit { get; set; }

        public bool FIDTurnOn { get; set; }

        public bool TKMaxRange { get; set; }

        public bool TypeLimit1 { get; set; }

        public bool TypeLimit2 { get; set; }

        public bool TypeLimit3 { get; set; }

        public bool ReceiptLimit { get; set; }

    }
}
