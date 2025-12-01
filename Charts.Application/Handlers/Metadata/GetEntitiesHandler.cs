using Charts.Application.QueryAndCommands.Metadata;
using Charts.Domain.Contracts;
using Charts.Domain.Contracts.Metadata.Dtos;
using Charts.Domain.Interfaces;
using MediatR;

namespace Charts.Application.Handlers.Metadata
{
    public sealed class GetEntitiesHandler(ICurrentDb db, IEntityMetadataService meta)
        : IRequestHandler<GetEntitiesQuery, ApiResponse<IReadOnlyList<EntityDto>>>
    {
        public async Task<ApiResponse<IReadOnlyList<EntityDto>>> Handle(GetEntitiesQuery req, CancellationToken ct)
        {
            try
            {
                await using var con = await db.OpenConnectionAsync(ct);

                var result = await meta.GetEntitiesAsync( con, db.Provider, ct);

                return ApiResponse<IReadOnlyList<EntityDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<IReadOnlyList<EntityDto>>.Fail(ex.Message, ex);
            }
        }
    }

}
