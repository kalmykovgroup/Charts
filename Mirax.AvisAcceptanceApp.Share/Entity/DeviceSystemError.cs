using System.ComponentModel.DataAnnotations;

namespace Mirax.AvisAcceptanceApp.Models.Entity.ModBusDevice.PortableDeviceModel
{
    public class DeviceSystemError
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Время калибровки(err)
        /// </summary>
        public bool ErrCalibInterval { get; set; }

        /// <summary>
        /// Время BUMP TEST(err)
        /// </summary>
        public bool ErrBumpInterval { get; set; }

        /// <summary>
        /// Критично низкий заряд
        /// </summary>
        public bool CriticalLowBattery { get; set; }

        /// <summary>
        /// Ошибка микросхемы at45 (память)(err)
        /// </summary>
        public bool ErrAT45 { get; set; }

        /// <summary>
        /// Ошибка микросхемы ADC
        /// </summary>
        public bool CommonErrADC { get; set; }

        /// <summary>
        /// Ошибка микросхемы BLE
        /// </summary>
        public bool ErrBLE { get; set; }

        /// <summary>
        /// Ошибка микросхемы GPS
        /// </summary>
        public bool ErrGPS { get; set; }

        /// <summary>
        /// Ошибка микросхемы GSM
        /// </summary>
        public bool ErrGSM { get; set; }

        /// <summary>
        /// Ошибка микросхемы LORA
        /// </summary>
        public bool ErrLORA { get; set; }

        /// <summary>
        /// Ошибка LCD
        /// </summary>
        public bool ErrLCD { get; set; }

        /// <summary>
        /// Критическое состояние батареи
        /// </summary>
        public bool CriticalBattery { get; set; }

    }
}
