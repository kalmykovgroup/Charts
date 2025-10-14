using Charts.Api.Application.Interfaces;
using Charts.Api.Application.Contracts;
using Charts.Api.Application.Contracts.Metadata.Dtos;
using Charts.Api.Application.Interfaces;
using Charts.Api.Application.QueryAndCommands.Metadata;
using MediatR;

namespace Charts.Api.Application.Handlers.Metadata
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
