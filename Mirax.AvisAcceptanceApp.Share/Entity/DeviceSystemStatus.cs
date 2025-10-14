using System.ComponentModel.DataAnnotations;

namespace Mirax.AvisAcceptanceApp.Models.Entity.ModBusDevice.PortableDeviceModel
{
    public class DeviceSystemStatus
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Блокировка звука
        /// </summary>
        public bool BlockSound { get; set; }

        /// <summary>
        /// Блокировка звука при калибровке
        /// </summary>
        public bool BlockSoundLimitCalib { get; set; }

        /// <summary>
        /// Блокировка выключения
        /// </summary>
        public bool BlockTurnOff { get; set; }

        /// <summary>
        /// Блокировка калибровки через меню
        /// </summary>
        public bool BlockCalib { get; set; }

        /// <summary>
        /// Прибор выключен ( не ведутся логи в режиме выключения )
        /// </summary>
        public bool TurnOff { get; set; }

        /// <summary>
        /// Режим обмена данными
        /// </summary>
        public bool DataExchange { get; set; }

        /// <summary>
        /// Режим зарядки устройства
        /// </summary>
        public bool BatCharge { get; set; }

        /// <summary>
        /// BUMP TEST
        /// </summary>
        public bool BumpTest { get; set; }

        /// <summary>
        /// Старт BUMP TEST
        /// </summary>
        public bool BumpTestRun { get; set; }

        /// <summary>
        /// Отображение единиц измерения на экране
        /// </summary>
        public bool LcdUnit { get; set; }

        /// <summary>
        /// Включение BLE
        /// </summary>
        public bool TurnOnBLE { get; set; }

        /// <summary>
        /// Включение Green Run LED
        /// </summary>
        public bool TurnStateRunLED { get; set; }

        /// <summary>
        /// Тест звука
        /// </summary>
        public bool TurnTestSound { get; set; }

        /// <summary>
        /// Включение GPS
        /// </summary>
        public bool TurnOnGPS { get; set; }

        /// <summary>
        /// Включение GSM
        /// </summary>
        public bool TurnOnGSM { get; set; }

        /// <summary>
        /// Включение LORA
        /// </summary>
        public bool TurnOnLORA { get; set; }
    }
}
