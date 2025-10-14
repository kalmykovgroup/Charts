using Charts.Api.Application.Contracts;
using Charts.Api.Application.Interfaces;
using Charts.Api.Application.QueryAndCommands.Mirax;
using Charts.Api.Domain.Interfaces.Mirax;
using Charts.Api.Domain.Mirax;
using MediatR;

namespace Charts.Api.Application.Handlers.Mirax
{
    // Handler  Получить устройства для испытания
    public sealed class GetPortableDevicesHandler : IRequestHandler<GetPortableDevicesQuery, ApiResponse<List<PortableDeviceDto>>>
    {
        private readonly ICurrentDb _db;
        private readonly IMiraxRepository _repository;

        public GetPortableDevicesHandler(ICurrentDb db, IMiraxRepository repository)
        {
            _db = db;
            _repository = repository;
        }

        public async Task<ApiResponse<List<PortableDeviceDto>>> Handle(
            GetPortableDevicesQuery query,
            CancellationToken ct)
        {
            try
            {
                await using var con = await _db.OpenConnectionAsync(ct);

                var items = await _repository.GetPortableDevicesAsync(
                    con,
                    _db.Provider,
                    query.TechnicalRunId,
                    ct
                );

                return ApiResponse<List<PortableDeviceDto>>.Ok(items);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<PortableDeviceDto>>.Fail(ex.Message, ex);
            }
        }
    }
}
