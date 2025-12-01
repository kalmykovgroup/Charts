using Charts.Application.QueryAndCommands.Metadata;
using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Metadata.Dtos;
using Charts.Domain.Interfaces;
using MediatR;

namespace Charts.Application.Handlers.Metadata
{
    public sealed class GetEntityFieldsHandler(ICurrentDb db, IEntityMetadataService meta) : IRequestHandler<GetEntityFieldsQuery, ApiResponse<IReadOnlyList<FieldDto>>>
    {
        public async Task<ApiResponse<IReadOnlyList<FieldDto>>> Handle(GetEntityFieldsQuery req, CancellationToken ct)
        {
            await using var con = await db.OpenConnectionAsync(ct);
            var (_, _, _, fields) = await meta.GetEntityFieldsAsync(con, db.Provider, req.Entity, ct);
            return ApiResponse<IReadOnlyList<FieldDto>>.Ok(fields);
        }
    }
}
