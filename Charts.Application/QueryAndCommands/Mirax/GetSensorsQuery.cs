using Charts.Domain.Contracts;
using Charts.Domain.Mirax;
using MediatR;

namespace Charts.Application.QueryAndCommands.Mirax
{
    // Получить сенсоры для устройства
    public sealed record GetSensorsQuery(Guid TechnicalRunId, string FactoryNumber)
        : IRequest<ApiResponse<List<SensorDto>>>;
}
