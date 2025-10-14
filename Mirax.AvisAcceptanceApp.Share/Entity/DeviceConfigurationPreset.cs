using Mirax.AvisAcceptanceApp.Share.Entity;
using System.ComponentModel.DataAnnotations;

namespace Mirax.AvisAcceptanceApp.Models.Entity.Configuration
{
    public class DeviceConfigurationPreset
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Название предустановки
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Название модификаций прибора
        /// </summary>
        public required string DeviceName { get; set; }

        /// <summary>
        /// Блютуз включён\не выключен
        /// </summary>
        public bool? isBluetooth { get; set; }

        /// <summary>
        /// Блютуз виден\не виден
        /// </summary>
        public bool? isBluetoothEnable { get; set; }

        /// <summary>
        /// Аппаратная версия
        /// </summary>
        public float? HardwareVersion { get; set; }

        /// <summary>
        /// Калибровочная температура
        /// </summary>  
        public float? CalibrationTemperature { get; set; }

        /// <summary>
        /// Уровень звука
        /// </summary>
        public int? SoundLevel { get; set; }

        /// <summary>
        /// Флаг записи в архив
        /// </summary>
        public bool? isArchiveRecord { get; set; }

        /// <summary>
        /// Интервал записи архивов
        /// </summary>
        public int? ArchiveRecordingInterval { get; set; }


        /// <summary>
        /// Интервал записи архива в режиме порогов
        /// </summary>
        public int? ArchiveRecordingIntervalThresholdMode { get; set; }

        /// <summary>
        /// Дата производства
        /// </summary>
        public DateTime? ProductionDate { get; set; }

        /// <summary>
        /// Отображение единиц измерений в приборе
        /// </summary>
        public bool? IsLCDActive { get; set; }

        /// <summary>
        /// LORA
        /// </summary>
        public bool? IsLORAActive { get; set; }

        /// <summary>
        /// GPS
        /// </summary>
        public bool? IsGPSActive { get; set; }

        /// <summary>
        /// GSM
        /// </summary>
        public bool? IsGSMACtive { get; set; }

        public DeviceConfigurationChanelState ChanelState1 { get; set; } = new();
        public DeviceConfigurationChanelState ChanelState2 { get; set; } = new();
        public DeviceConfigurationChanelState ChanelState3 { get; set; } = new();
        public DeviceConfigurationChanelState ChanelState4 { get; set; } = new();
        public DeviceConfigurationChanelState ChanelState5 { get; set; } = new();
        public DeviceConfigurationChanelState ChanelState6 { get; set; } = new();

    }

}
