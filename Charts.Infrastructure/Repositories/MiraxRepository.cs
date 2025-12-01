using System.Data.Common;
using Charts.Domain.Contracts.Types;
using Charts.Domain.Interfaces.Mirax;
using Charts.Domain.Mirax;
using Dapper;

namespace Charts.Infrastructure.Repositories
{
 

    public sealed class MiraxRepository : IMiraxRepository
    {
        public async Task<List<TechnicalRunToStartDto>> GetTechnicalRunsAsync(
            DbConnection connection,
            DbProviderType provider,
            string? factoryNumber,
            CancellationToken cancellationToken)
        {
            ValidateProvider(provider);

            string sql;
            object? parameters = null;

            if (string.IsNullOrWhiteSpace(factoryNumber))
            {
                // Если factoryNumber не передан - возвращаем все испытания
                sql = @"
            SELECT 
                ""Id"",
                ""Name"",
                ""DateStarTime"",
                ""DateEndTime""
            FROM ""TechnicalRunsToStart""
            ORDER BY ""Id"" ASC";
            }
            else
            {
                // Если factoryNumber передан - фильтруем только испытания с этим устройством
                sql = @"
            SELECT 
                t.""Id"",
                t.""Name"",
                t.""DateStarTime"",
                t.""DateEndTime""
            FROM ""TechnicalRunsToStart"" t
            WHERE EXISTS (
                SELECT 1 
                FROM ""DeviceEntity"" d
                WHERE d.""TechnicalRunToStartId"" = t.""Id""
                  AND d.""FactoryNumber"" = @FactoryNumber
            )
            ORDER BY t.""Id"" ASC";

                parameters = new { FactoryNumber = factoryNumber };
            }

            var result = await connection.QueryAsync<TechnicalRunToStartDto>(
                     new CommandDefinition(sql, parameters, cancellationToken: cancellationToken)
                 ); 

            return result.AsList();
        }

        public async Task<List<PortableDeviceDto>> GetPortableDevicesAsync(
            DbConnection connection,
            DbProviderType provider,
            Guid technicalRunId,
            CancellationToken cancellationToken)
        {
            ValidateProvider(provider);

            const string sql = @"
                SELECT DISTINCT ON (""FactoryNumber"")
                    ""Id"",
                    ""FactoryNumber"",
                    ""Name"",
                    ""ComPortName""
                FROM ""DeviceEntity"" 
                WHERE ""Discriminator"" = 'PortableDevice' 
                    AND ""TechnicalRunToStartId"" = @TechnicalRunId
                ORDER BY ""FactoryNumber"", ""Id""";

            var result = await connection.QueryAsync<PortableDeviceDto>(
                new CommandDefinition(
                    sql,
                    new { TechnicalRunId = technicalRunId },
                    cancellationToken: cancellationToken
                )
            );

            return result.AsList();
        }

        public async Task<List<SensorDto>> GetSensorsAsync(
            DbConnection connection,
            DbProviderType provider,
            Guid technicalRunId,
            string factoryNumber,
            CancellationToken cancellationToken)
        {
            ValidateProvider(provider);

            const string sql = @"
                SELECT DISTINCT ON (""Modification"")
                    ""Id"",
                    ""Gas"",
                    ""ChannelNumber"",
                    ""DisplayUnits"",
                    ""MainUnits"",
                    ""Modification""
                FROM ""DeviceEntity"" 
                WHERE ""Discriminator"" = 'Sensor' 
                    AND ""FactoryNumber"" = @FactoryNumber
                    AND ""TechnicalRunToStartId"" = @TechnicalRunId
                ORDER BY ""Modification"", ""FactoryNumber"", ""Id""";

            var result = await connection.QueryAsync<SensorDto>(
                new CommandDefinition(
                    sql,
                    new { FactoryNumber = factoryNumber, TechnicalRunId = technicalRunId },
                    cancellationToken: cancellationToken
                )
            );

            return result.AsList();
        }

        private static void ValidateProvider(DbProviderType provider)
        {
            if (provider != DbProviderType.PostgreSql)
            {
                throw new NotImplementedException(
                    $"Provider {provider} is not implemented. Only PostgreSQL is supported.");
            }
        }
    }
}