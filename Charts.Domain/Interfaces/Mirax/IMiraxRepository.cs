using Charts.Api.Domain.Contracts.Types;
using Charts.Api.Domain.Mirax;
using System.Data.Common;

namespace Charts.Api.Domain.Interfaces.Mirax
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
