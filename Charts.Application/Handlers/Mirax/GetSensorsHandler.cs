using Charts.Application.QueryAndCommands.Mirax;
using Charts.Domain.Contracts;
using Charts.Domain.Interfaces;
using Charts.Domain.Interfaces.Mirax;
using Charts.Domain.Mirax;
using MediatR;

namespace Charts.Application.Handlers.Mirax
{

    // Handler Получить сенсоры для устройства
    public sealed class GetSensorsHandler : IRequestHandler<GetSensorsQuery, ApiResponse<List<SensorDto>>>
    {
        private readonly ICurrentDb _db;
        private readonly IMiraxRepository _repository;

        public GetSensorsHandler(ICurrentDb db, IMiraxRepository repository)
        {
            _db = db;
            _repository = repository;
        }

        public async Task<ApiResponse<List<SensorDto>>> Handle(
            GetSensorsQuery query,
            CancellationToken ct)
        {
            await using var con = await _db.OpenConnectionAsync(ct);
            var items = await _repository.GetSensorsAsync(
                con,
                _db.Provider,
                query.TechnicalRunId,
                query.FactoryNumber,
                ct
            );
            return ApiResponse<List<SensorDto>>.Ok(items);
        }
    }
}
