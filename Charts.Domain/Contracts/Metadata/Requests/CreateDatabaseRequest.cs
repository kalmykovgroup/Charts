using Charts.Api.Application.Contracts.Metadata.Dtos;
using Charts.Api.Application.Contracts.Template.Dtos;
using Charts.Api.Application.Models;
using Charts.Api.Domain.Contracts.Types;

namespace Charts.Api.Application.Contracts.Metadata.Requests
{
    public class CreateDatabaseRequest
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DatabaseVersion { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;

        public DbProviderType Provider { get; set; } = DbProviderType.PostgreSql;
        public DatabaseStatus DatabaseStatus { get; set; }  
    }
}
