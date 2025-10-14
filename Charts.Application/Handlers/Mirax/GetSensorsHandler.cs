using Charts.Api.Application.Contracts;
using Charts.Api.Application.Interfaces;
using Charts.Api.Application.QueryAndCommands.Mirax;
using Charts.Api.Domain.Interfaces.Mirax;
using Charts.Api.Domain.Mirax;
using MediatR;

namespace Charts.Api.Application.Handlers.Mirax
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
            try
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
            catch (Exception ex)
            {
                return ApiResponse<List<SensorDto>>.Fail(ex.Message, ex);
            }
        }
    }
}
