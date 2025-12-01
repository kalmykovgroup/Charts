using Charts.Domain.Contracts;
using Charts.Domain.Mirax;
using MediatR;

namespace Charts.Application.QueryAndCommands.Mirax
{
    // Получить устройства для испытания
    public sealed record GetPortableDevicesQuery(Guid TechnicalRunId)
        : IRequest<ApiResponse<List<PortableDeviceDto>>>;
}
