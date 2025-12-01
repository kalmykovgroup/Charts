using Charts.Application.QueryAndCommands.Mirax;
using Charts.Domain.Contracts;
using Charts.Domain.Interfaces;
using Charts.Domain.Interfaces.Mirax;
using Charts.Domain.Mirax;
using MediatR;

namespace Charts.Application.Handlers.Mirax
{

    // Handler Получить все испытания
    public sealed class GetTechnicalRunsHandler : IRequestHandler<GetTechnicalRunsQuery, ApiResponse<List<TechnicalRunToStartDto>>>
    {
        private readonly ICurrentDb _db;
        private readonly IMiraxRepository _repository;

        public GetTechnicalRunsHandler(ICurrentDb db, IMiraxRepository repository)
        {
            _db = db;
            _repository = repository;
        }

        public async Task<ApiResponse<List<TechnicalRunToStartDto>>> Handle(
            GetTechnicalRunsQuery query,
            CancellationToken ct)
        {
            await using var con = await _db.OpenConnectionAsync(ct);
            var items = await _repository.GetTechnicalRunsAsync(con, _db.Provider, query.FactoryNumber, ct);
            return ApiResponse<List<TechnicalRunToStartDto>>.Ok(items);
        }
    }
}
