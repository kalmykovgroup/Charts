using System.Data.Common;
using Charts.Domain.Contracts.Metadata.Dtos;
using Charts.Domain.Contracts.Types;

namespace Charts.Domain.Interfaces;

public interface IEntityMetadataService
{
    Task<IReadOnlyList<EntityDto>> GetEntitiesAsync(DbConnection con, DbProviderType provider, CancellationToken ct);
    Task<(EntityDto Entity, string Schema, string Table, IReadOnlyList<FieldDto> Fields)> GetEntityFieldsAsync(DbConnection con, DbProviderType provider, string entity, CancellationToken ct);
    (string Schema, string Table, string TimeColumn, string ValueColumn) ValidateAndResolveColumns((string Schema, string Table, IReadOnlyList<FieldDto> Fields) meta, string? timeField, string valueField);
}
