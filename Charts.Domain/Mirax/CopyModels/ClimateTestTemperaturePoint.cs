using System.ComponentModel.DataAnnotations;

namespace Charts.Api.Domain.Mirax.CopyModels
{
    public class ClimateTestTemperaturePoint 
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Температура
        /// </summary> 
        public double Temperature { get; set; }

        /// <summary>
        /// Рассчетное время перехода с предыдущей точки на текущую
        /// </summary> 
        public TimeSpan TransitionTime { get; set; }

        /// <summary>
        /// Время температурной стабилизации
        /// </summary> 
        public TimeSpan HoldingTime { get; set; }

        /// <summary>
        /// Время подачи всех газов на температурной точке
        /// </summary> 
        public TimeSpan GasTime { get; set; }

        /// <summary>
        /// Номер по порядку
        /// </summary> 
        public int Position { get; set; }

        /// <summary>
        /// Время начала точки
        /// </summary> 
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Время начала температурной стабилизации
        /// </summary> 
        public DateTime StabilizationTime { get; set; }

        /// <summary>
        /// Время начала подачи газов
        /// </summary> 
        public DateTime GasStartTime { get; set; }

        /// <summary>
        /// Время окончания подачи газов
        /// </summary> 
        public DateTime GasEndTime { get; set; }

    }
}
