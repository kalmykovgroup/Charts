using System.Data.Common;
using Charts.Domain.Contracts.Types;
using Charts.Domain.Mirax;

namespace Charts.Domain.Interfaces.Mirax
{

    public interface IMiraxRepository
    {
        /// <summary>
        /// Получить список всех испытаний без устройств
        /// </summary>
        Task<List<TechnicalRunToStartDto>> GetTechnicalRunsAsync(
            DbConnection connection,
            DbProviderType provider,
            string? factoryNumber,
            CancellationToken cancellationToken);

        /// <summary>
        /// Получить список устройств для конкретного испытания без сенсоров
        /// </summary>
        Task<List<PortableDeviceDto>> GetPortableDevicesAsync(
            DbConnection connection,
            DbProviderType provider,
            Guid technicalRunId,
            CancellationToken cancellationToken);

        /// <summary>
        /// Получить список сенсоров для конкретного устройства в испытании
        /// </summary>
        Task<List<SensorDto>> GetSensorsAsync(
            DbConnection connection,
            DbProviderType provider,
            Guid technicalRunId,
            string factoryNumber,
            CancellationToken cancellationToken);
    }
}
