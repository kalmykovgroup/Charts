using Charts.Api.Application.Contracts;
using Charts.Api.Domain.Mirax;
using MediatR;

namespace Charts.Api.Application.QueryAndCommands.Mirax
{
    // Получить сенсоры для устройства
    public sealed record GetSensorsQuery(Guid TechnicalRunId, string FactoryNumber)
        : IRequest<ApiResponse<List<SensorDto>>>;
}
