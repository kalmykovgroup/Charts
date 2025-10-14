using Charts.Api.Application.Contracts.Template.Dtos;
using Charts.Api.Application.Models;
using Charts.Api.Domain.Contracts.Types;

namespace Charts.Api.Application.Contracts.Metadata.Dtos
{
    public class DatabaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DatabaseVersion { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty; 
        public string? Description { get; set; } = null; 

        public DbProviderType Provider { get; set; } = DbProviderType.PostgreSql;
        public DatabaseStatus DatabaseStatus { get; set; }
        public DatabaseAvailability Availability { get; set; } = DatabaseAvailability.Unknown;
        public DateTime? LastConnectivityAt { get; set; }
        public string? LastConnectivityError { get; set; }

        public List<EntityDto> Entities { get; set; } = [];
        public List<ChartReqTemplateDto> ChartReqTemplates { get; set; } = [];
    }
}
