using Charts.Api.Application.Contracts;
using Charts.Api.Domain.Mirax;
using MediatR;

namespace Charts.Api.Application.QueryAndCommands.Mirax
{
    // Получить устройства для испытания
    public sealed record GetPortableDevicesQuery(Guid TechnicalRunId)
        : IRequest<ApiResponse<List<PortableDeviceDto>>>;
}
