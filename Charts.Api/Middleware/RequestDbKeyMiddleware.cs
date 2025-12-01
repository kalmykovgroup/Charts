using Charts.Domain.Interfaces;

namespace Charts.Api.Middleware;
// Middleware/RequestDbKeyMiddleware.cs
public sealed class RequestDbKeyMiddleware : IMiddleware
{
    private readonly IRequestDbKeyAccessor _acc;
    public RequestDbKeyMiddleware(IRequestDbKeyAccessor acc) => _acc = acc;

    public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
    {
        // 1) парсим GUID из заголовка/квери
        var idText = ctx.Request.Headers["X-Db"].FirstOrDefault()
                  ?? ctx.Request.Query["db"].FirstOrDefault();

        if (Guid.TryParse(idText, out var id))
            _acc.Set(id);
        else
            _acc.Set(null);

        // 2) смотреть атрибут нужно ПОСЛЕ маршрутизации
        var required = ctx.GetEndpoint()?.Metadata.GetMetadata<RequireDbKeyAttribute>() is not null;

        if (required && _acc.DbId is null)
        {
            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            await ctx.Response.WriteAsJsonAsync(new
            {
                error = "Missing database id. Send header 'X-Db: <guid>' or query '?db=<guid>'."
            });
            return;
        }

        await next(ctx);
    }
}

public static class RequestDbKeyMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestDbKey(this IApplicationBuilder app)
        => app.UseMiddleware<RequestDbKeyMiddleware>();
}
 

