using Mirax.AvisAcceptanceApp.Share.Types;
using System.ComponentModel.DataAnnotations;

namespace Mirax.AvisAcceptanceApp.Models.Entity.Configuration
{
    public class SensorConfigurationPreset : ICloneable
    {
        [Key]
        public Guid Id { get; set; }

        public bool isChanelActive { get; set; } = false; 

        public string? LCDFormula { get; set; }

        public double? MaximumConcentration { get; set; }

        /// <summary>
        /// Название предустановки
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Название прибора
        /// </summary>
        public required string DeviceName { get; set; }

        public SenerType SensorModification { get; set; }

        /// <summary>
        /// Гистерезис
        /// </summary>
        public double? Hysteresis { get; set; }

        /// <summary>
        /// Фильтрация
        /// </summary>
        public int? Filtration { get; set; }

        /// <summary>
        /// Верхнее значение диапазона
        /// </summary>
        public double? UpperRangeValue { get; set; }

        public double? DeadZone1 { get; set; }
        public double? DeadZone2 { get; set; }

        #region NKPR

        /// <summary>
        /// НКПР коэффициент
        /// </summary>
        public double? NKPRCoefficient { get; set; }

        /// <summary>
        /// Молекулярная масса
        /// </summary>
        public double? NKPRMGM { get; set; }

        #endregion

        #region Threshold
        public double? Threshold1 { get; set; }

        public double? Threshold2 { get; set; }

        public double? Threshold3 { get; set; }

        public double? Threshold4 { get; set; }

        public double? Threshold5 { get; set; }

        #endregion

        #region Linearization
        public double? LinearizationConsentration1 { get; set; }
        public double? LinearizationCoefficient1 { get; set; }

        public double? LinearizationConsentration2 { get; set; }
        public double? LinearizationCoefficient2 { get; set; }

        public double? LinearizationConsentration3 { get; set; }
        public double? LinearizationCoefficient3 { get; set; }

        public double? LinearizationConsentration4 { get; set; }
        public double? LinearizationCoefficient4 { get; set; }

        public double? LinearizationConsentration5 { get; set; }
        public double? LinearizationCoefficient5 { get; set; }

        public double? LinearizationConsentration6 { get; set; }
        public double? LinearizationCoefficient6 { get; set; }

        public double? LinearizationConsentration7 { get; set; }
        public double? LinearizationCoefficient7 { get; set; }

        public double? LinearizationConsentration8 { get; set; }
        public double? LinearizationCoefficient8 { get; set; }

        public double? LinearizationConsentration9 { get; set; }
        public double? LinearizationCoefficient9 { get; set; }

        public double? LinearizationConsentration10 { get; set; }
        public double? LinearizationCoefficient10 { get; set; }

        #endregion

        #region Temperature
        public double? TemperatureValue1 { get; set; }
        public double? TemperatureCoefficient1 { get; set; }

        public double? TemperatureValue2 { get; set; }
        public double? TemperatureCoefficient2 { get; set; }

        public double? TemperatureValue3 { get; set; }
        public double? TemperatureCoefficient3 { get; set; }

        public double? TemperatureValue4 { get; set; }
        public double? TemperatureCoefficient4 { get; set; }

        public short? TemperatureValue5 { get; set; }
        public double? TemperatureCoefficient5 { get; set; }

        public double? TemperatureValue6 { get; set; }
        public double? TemperatureCoefficient6 { get; set; }

        public double? TemperatureValue7 { get; set; }
        public double? TemperatureCoefficient7 { get; set; }

        public double? TemperatureValue8 { get; set; }
        public double? TemperatureCoefficient8 { get; set; }

        public double? TemperatureValue9 { get; set; }
        public double? TemperatureCoefficient9 { get; set; }

        public double? TemperatureValue10 { get; set; }
        public double? TemperatureCoefficient10 { get; set; }

        #endregion

        #region Null Temperature
        public double? NullTemperatureValue1 { get; set; }
        public double? NullTemperatureCoefficient1 { get; set; }

        public double? NullTemperatureValue2 { get; set; }
        public double? NullTemperatureCoefficient2 { get; set; }

        public double? NullTemperatureValue3 { get; set; }
        public double? NullTemperatureCoefficient3 { get; set; }

        public double? NullTemperatureValue4 { get; set; }
        public double? NullTemperatureCoefficient4 { get; set; }

        public double? NullTemperatureValue5 { get; set; }
        public double? NullTemperatureCoefficient5 { get; set; }

        public double? NullTemperatureValue6 { get; set; }
        public double? NullTemperatureCoefficient6 { get; set; }

        public double? NullTemperatureValue7 { get; set; }
        public double? NullTemperatureCoefficient7 { get; set; }

        public double? NullTemperatureValue8 { get; set; }
        public double? NullTemperatureCoefficient8 { get; set; }

        public double? NullTemperatureValue9 { get; set; }
        public double? NullTemperatureCoefficient9 { get; set; }

        public double? NullTemperatureValue10 { get; set; }
        public double? NullTemperatureCoefficient10 { get; set; }

        #endregion




        #region Discretization

        public bool IsNeedToDiscreteness { get; set; } = false;

        public UnitsOfMeasurementEnum? DDiscretization { get; set; }

        public int? MeasureConcDisplay { get; set; }

        public UnitsOfMeasurementEnum? BDiscretization { get; set; }

        public int? MeasureConcBasic { get; set; }

        #endregion

        #region FirstMeasuringCircuit

        public int? OffsetB { get; set; }

        public int? OffsetZ { get; set; }

        public PGAEnum? PGA { get; set; }

        public RGAINEnum? RGAIN { get; set; }

        #endregion


        #region TK параметры

        /// <summary>
        /// Время прогрева сенсоар в секундах
        /// </summary>
        public int? SensorWarmUpTime { get; set; }

        /// <summary>
        /// Автокалибровка сенсора при включений
        /// </summary>
        public bool? AutoСalibrationWarmSensor { get; set; }

        /// <summary>
        /// Ограничение калибровки диапазона
        /// </summary>
        public bool? IsRangeСalibrationLimitation { get; set; }

        /// <summary>
        /// Ограничение калибровки диапазона в процентах
        /// </summary>
        public int? RangeСalibrationLimitation { get; set; }

        #endregion

        #region Тип сенсора

        public SensorType? SensorType { get; set; }

        public UartSensorType? UartSensorType { get; set; }

        public EcSensorSubtype? EcSensorSubtype { get; set; }

        public O2SensorSubtype? O2SensorSubtype { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

    }
}
