using Charts.Api.Middleware;
using Charts.Application.QueryAndCommands.Metadata;
using Charts.Application.QueryAndCommands.Metadata.Databases;
using Charts.Domain.Contracts.Metadata.Requests;
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

    [HttpPost("database/create")]
    public async Task<IActionResult> CreateDatabase([FromBody] CreateDatabaseRequest body, CancellationToken ct)
    {
        var created = await mediator.Send(new CreateDatabaseCommand(body), ct);
        return Ok(created);
    }

    [HttpPut("database/update/{id}")]
    public async Task<IActionResult> UpdateDatabase([FromRoute] Guid id, [FromBody] UpdateDatabaseRequest body, CancellationToken ct)
    {
        if (body.Id == Guid.Empty || body.Id != id)
            return BadRequest(new { message = "Body.id должен совпадать с маршрутом /databases/update/{id}." });

        var updated = await mediator.Send(new UpdateDatabaseCommand(id, body), ct);
        return Ok(updated);
    }

    [HttpDelete("database/delete/{id}")]
    public async Task<IActionResult> DeleteDatabase([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteDatabaseCommand(id), ct);
        return Ok(result);
    }

    [HttpPost("database/test-connection")]
    public async Task<IActionResult> TestConnection([FromBody] TestConnectionRequest body, CancellationToken ct)
    {
        var result = await mediator.Send(new TestConnectionCommand(body.ConnectionString, body.Provider), ct);
        return Ok(result);
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

