using Charts.Api.Application.Contracts;
using Charts.Api.Application.Contracts.Metadata.Dtos;
using Charts.Api.Domain.Contracts.Types;

namespace Charts.Api.Application.Models
{
    public class Database : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string DatabaseVersion { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string? Description { get; set; } = null;


        public DbProviderType Provider { get; set; } = DbProviderType.PostgreSql;
        public DatabaseStatus DatabaseStatus {  get; set; }
        public DatabaseAvailability Availability { get; set; } = DatabaseAvailability.Unknown;
        public DateTimeOffset? LastConnectivityAt { get; set; }
        public string? LastConnectivityError { get; set; }

        public List<EntityDto> Entities { get; set; } = [];

        public List<ChartReqTemplate> ChartReqTemplates { get; set; } = [];
    }
}
