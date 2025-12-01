using Charts.Api.Middleware;
using Charts.Application.QueryAndCommands.Mirax;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Charts.Api.Controllers
{
    [ApiController]
    [Route("mirax/")]
    public class MiraxController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MiraxController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получить список всех испытаний
        /// </summary>
        [RequireDbKey]
        [HttpGet("technical-runs")]
        public async Task<IActionResult> GetTechnicalRuns([FromQuery] string? factoryNumber, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetTechnicalRunsQuery(factoryNumber), ct);
            return Ok(result);
        }

        /// <summary>
        /// Получить список устройств для конкретного испытания
        /// </summary>
        /// <param name="technicalRunId">ID испытания</param>
        [RequireDbKey]
        [HttpGet("technical-runs/{technicalRunId:guid}/devices")]
        public async Task<IActionResult> GetPortableDevices(
            [FromRoute] Guid technicalRunId,
            CancellationToken ct)
        {
            var result = await _mediator.Send(
                new GetPortableDevicesQuery(technicalRunId),
                ct
            );
            return Ok(result);
        }

        /// <summary>
        /// Получить список сенсоров для конкретного устройства в испытании
        /// </summary>
        /// <param name="technicalRunId">ID испытания</param>
        /// <param name="factoryNumber">Заводской номер устройства</param>
        [RequireDbKey]
        [HttpGet("technical-runs/{technicalRunId:guid}/devices/{factoryNumber}/sensors")]
        public async Task<IActionResult> GetSensors([FromRoute] Guid technicalRunId, [FromRoute] string factoryNumber,
            CancellationToken ct)
        {
            var result = await _mediator.Send(
                new GetSensorsQuery(technicalRunId, factoryNumber),
                ct
            );
            return Ok(result);
        }

     
    }
}
