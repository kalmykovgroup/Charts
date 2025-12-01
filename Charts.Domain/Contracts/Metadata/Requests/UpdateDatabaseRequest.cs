using Charts.Domain.Contracts.Types;

namespace Charts.Domain.Contracts.Metadata.Requests
{
    public class UpdateDatabaseRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DatabaseVersion { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;

        public DbProviderType Provider { get; set; } = DbProviderType.PostgreSql;
        public EntityStatus Status { get; set; } = EntityStatus.Active;
    }
}
