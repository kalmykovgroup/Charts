using Charts.Api.Application.Contracts;
using Charts.Api.Application.Contracts.Charts.Dtos;
using Charts.Api.Application.Contracts.Charts.Requests;
using Charts.Api.Application.Contracts.Charts.Responces;
using Charts.Api.Application.QueryAndCommands.Chart;
using Charts.Api.Middleware;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Charts.Api.Controllers;


[ApiController]
[Route("charts/")]
public class ChartsController(IMediator mediator, ILogger<ChartsController> logger) : ControllerBase
{ 

    [RequireDbKey]
    [HttpPost]
    [Route("multi")]
    public async Task<IActionResult> Multi([FromBody] GetMultiSeriesRequest body, CancellationToken ct)
    {
        ApiResponse<MultiSeriesResponse> result = await mediator.Send(new GetMultiSeriesQuery(body), ct);
        if (result.Success)
        {
            List<SeriesBinDto> s = new List<SeriesBinDto>();

            foreach (var item in result.Data.Series)
            {
                foreach (var bin in item.Bins)
                {
                    if (bin.Min == null && bin.Max != null)
                        s.Add(bin);
                }
            }

            if (s.Count > 0) {
                logger.LogInformation($"MultiSeriesItemDto: {s.Count}");
            }


            logger.LogInformation("Запрос успешно был выполнен");
        }
        else
        {
            logger.LogWarning("Возможно запрос был отменен");
        }
          
        return Ok(result);
    } 
}
