namespace Ketabi.Web.Middlewares;

public sealed class UnderDevelopmentMiddleware
{
    private readonly RequestDelegate _next;
    private const string UnderDevelopmentPath = "/Maintenance/UnderDevelopment";

    public UnderDevelopmentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.Equals(UnderDevelopmentPath, StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        if (!HttpMethods.IsGet(context.Request.Method) && !HttpMethods.IsHead(context.Request.Method))
        {
            await _next(context);
            return;
        }

        if (context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase) ||
            context.Request.Path.StartsWithSegments("/hubs", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        if (Path.HasExtension(context.Request.Path.Value ?? string.Empty))
        {
            await _next(context);
            return;
        }

        var endpoint = context.GetEndpoint();
        if (endpoint != null)
        {
            await _next(context);
            return;
        }

        context.Response.Redirect(UnderDevelopmentPath, permanent: false);
    }
}

public static class UnderDevelopmentMiddlewareExtensions
{
    public static IApplicationBuilder UseUnderDevelopmentGuard(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UnderDevelopmentMiddleware>();
    }
}
