using Charts.Api.Middleware;
using Charts.Application.QueryAndCommands.Metadata;
using Charts.Application.QueryAndCommands.Metadata.Databases;
using Charts.Application.QueryAndCommands.Template;
using Charts.Domain.Contracts.Template.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Charts.Api.Controllers;


[ApiController]
[Route("metadata/")]
public class MetadataController(IMediator mediator) : ControllerBase
{
    [HttpGet("databases/all")]
    public async Task<IActionResult> GetDatabases(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllDatabasesQuery(), ct);
        return Ok(result);
    }


        // POST /templates/create
    [HttpPost("database/create")]
    public async Task<IActionResult> Create([FromBody] CreateChartReqTemplateRequest body, CancellationToken ct)
    {
         var created = await mediator.Send(new CreateChartReqTemplateCommand(body), ct); 
        return Ok(created); // фронт ждёт сам DTO
    }

    // PUT /templates/{id}
    [HttpPut("database/update/{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateChartReqTemplateRequest body, CancellationToken ct)
    {
        if (body.Id == Guid.Empty || body.Id != id)
            return BadRequest(new { message = "Body.id должен совпадать с маршрутом /templates/{id}." }); 

        var updated = await mediator.Send(new UpdateChartReqTemplateCommand(id, body), ct);
         
        return Ok(updated); // фронт ждёт сам DTO
    }

    // DELETE /templates/{id}
    [HttpDelete("database/delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    { 

        await mediator.Send(new DeleteChartReqTemplateCommand(id), ct);

        return Ok(new { ok = true }); // фронт ждёт именно { ok: true }
    }

    [RequireDbKey]
    [HttpGet("database/entities")]
    public async Task<IActionResult> GetEntities(CancellationToken ct)
    {
        var result = await mediator.Send(new GetEntitiesQuery(), ct);
        return Ok(result);
    }

    [RequireDbKey]
    [HttpGet("database/fields")]
    public async Task<IActionResult> GetFields([FromQuery] string entity, CancellationToken ct)
    {
        var result = await mediator.Send(new GetEntityFieldsQuery(entity), ct);
        return Ok(result);
    }
}
