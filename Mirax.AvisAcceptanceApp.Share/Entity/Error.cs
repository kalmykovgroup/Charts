using System.ComponentModel.DataAnnotations;

namespace Mirax.AvisAcceptanceApp.Models.Entity.ModBusDevice.PortableDeviceModel
{
    public class Error
    {

        [Key]
        public Guid Id { get; set; }

        public bool SensorFailed { get; set; }

        /// <summary>
        /// Ошибка микросхемы ADC
        /// </summary>
        public bool ErrADC { get; set; }

        /// <summary>
        /// Ошибка микросхемы mcp4652
        /// </summary>
        public bool ErrMCP4652 { get; set; }

        /// <summary>
        /// Ошибка микросхемы mcp47c
        /// </summary>
        public bool ErrMCP47 { get; set; }

        /// <summary>
        /// Порог 1 
        /// </summary>
        public bool Limit1 { get; set; }

        /// <summary>
        /// Порог 2
        /// </summary>
        public bool Limit2 { get; set; }

        /// <summary>
        /// Порог 3 
        /// </summary>
        public bool Limit3 { get; set; }

        /// <summary>
        /// Порог STEL 
        /// </summary>
        public bool LimitSTEL { get; set; }

        /// <summary>
        /// Порог TWA 
        /// </summary>
        public bool LimitTWA { get; set; }

        /// <summary>
        /// Превышение диапазона 
        /// </summary>
        public bool ExceededTheRange { get; set; }

        /// <summary>
        /// Ошибка при калибровке нуля(выставляется на третий раз как в ТЗ)
        /// </summary>
        public bool AutoZeroError { get; set; }

        /// <summary>
        /// Ошибка при калибровке диапазона(выставляется на третий раз как в ТЗ)
        /// </summary>
        public bool AutoSpanError { get; set; }

        /// <summary>
        /// Время калибровки(err) 
        /// </summary>
        public bool CalibInterval { get; set; }

    }
}
