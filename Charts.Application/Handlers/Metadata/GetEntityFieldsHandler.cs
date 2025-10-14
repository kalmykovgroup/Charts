using Charts.Api.Application.Interfaces;
using Charts.Api.Application.Contracts;
using Charts.Api.Application.Contracts.Metadata.Dtos;
using Charts.Api.Application.Interfaces;
using Charts.Api.Application.QueryAndCommands.Metadata;
using MediatR;

namespace Charts.Api.Application.Handlers.Metadata
{
    public sealed class GetEntityFieldsHandler(ICurrentDb db, IEntityMetadataService meta) : IRequestHandler<GetEntityFieldsQuery, ApiResponse<IReadOnlyList<FieldDto>>>
    {
        public async Task<ApiResponse<IReadOnlyList<FieldDto>>> Handle(GetEntityFieldsQuery req, CancellationToken ct)
        {

            try
            {
                await using var con = await db.OpenConnectionAsync(ct);

                var (_, _, _, fields) = await meta.GetEntityFieldsAsync( con, db.Provider, req.Entity, ct);

                return ApiResponse < IReadOnlyList < FieldDto >>.Ok(fields);
            }
            catch (Exception ex)
            {
                return ApiResponse<IReadOnlyList<FieldDto>>.Fail(ex.Message, ex);
            }
            
        }
    }
}
