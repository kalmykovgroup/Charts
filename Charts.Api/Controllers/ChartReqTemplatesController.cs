using Charts.Application.QueryAndCommands.Template;
using Charts.Domain.Contracts.Template.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Charts.Api.Controllers;

[ApiController]
[Route("templates")]
public sealed class ChartReqTemplatesController(IMediator mediator) : ControllerBase
{
    // GET /templates/all
    [HttpGet("all")] 
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var list = await mediator.Send(new GetAllChartReqTemplatesQuery(), ct);
        return Ok(list); // фронт ждёт чистый массив без обёртки
    }

    // POST /templates/create
    [HttpPost("create")] 
    public async Task<IActionResult> Create([FromBody] CreateChartReqTemplateRequest body, CancellationToken ct)
    { 

        var created = await mediator.Send(new CreateChartReqTemplateCommand(body), ct);
         
        return Ok(created); // фронт ждёт сам DTO
    }

    // PUT /templates/{id}
    [HttpPut("update/{id}")] 
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateChartReqTemplateRequest body, CancellationToken ct)
    {
        if (body.Id == Guid.Empty || body.Id != id)
            return BadRequest(new { message = "Body.id должен совпадать с маршрутом /templates/{id}." }); 

        var updated = await mediator.Send(new UpdateChartReqTemplateCommand(id, body), ct); 
        return Ok(updated); // фронт ждёт сам DTO
    }

    // DELETE /templates/{id}
    [HttpDelete("delete/{id}")] 
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    { 

       var result =  await mediator.Send(new DeleteChartReqTemplateCommand(id), ct); 
        return Ok(result);
    }
}