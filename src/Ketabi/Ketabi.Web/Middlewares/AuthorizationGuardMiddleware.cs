using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Ketabi.Web.Middlewares;

public sealed class AuthorizationGuardMiddleware
{
    private readonly RequestDelegate _next;
    private const string UnauthorizedPath = "/Account/Unauthorized";

    public AuthorizationGuardMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only consider GET/HEAD for page navigation
        if (!HttpMethods.IsGet(context.Request.Method) && !HttpMethods.IsHead(context.Request.Method))
        {
            await _next(context);
            return;
        }

        // Ignore APIs, hubs and static files
        if (context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase) ||
            context.Request.Path.StartsWithSegments("/hubs", StringComparison.OrdinalIgnoreCase) ||
            Path.HasExtension(context.Request.Path.Value ?? string.Empty))
        {
            await _next(context);
            return;
        }

        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            await _next(context);
            return;
        }

        // Quick path: enforce admin/moderator for admin/dashboard routes
        var path = context.Request.Path;
        var endpointDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
        var isAdminRoute = path.StartsWithSegments("/Admin", StringComparison.OrdinalIgnoreCase)
                           || string.Equals(endpointDescriptor?.ControllerName, "Dashboard", StringComparison.OrdinalIgnoreCase)
                           || path.StartsWithSegments("/Dashboard", StringComparison.OrdinalIgnoreCase);

        if (isAdminRoute)
        {
            var userForAdmin = context.User;
            // Check authentication first
            if (userForAdmin?.Identity?.IsAuthenticated != true)
            {
                context.Response.Redirect(UnauthorizedPath);
                return;
            }

            // Then validate roles: only allow Admin or Moderator
            if (!userForAdmin.IsInRole("Admin") && !userForAdmin.IsInRole("Moderator"))
            {
                context.Response.Redirect(UnauthorizedPath);
                return;
            }
        }

        // If endpoint allows anonymous access, proceed
        if (endpoint.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            await _next(context);
            return;
        }

        // If endpoint does not require authorization, proceed
        var requiresAuth = endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>().Any();
        if (!requiresAuth)
        {
            await _next(context);
            return;
        }

        var user = context.User;
        // If not authenticated, redirect to Unauthorized (could be login flow but requirement is unified page)
        if (user?.Identity?.IsAuthenticated != true)
        {
            context.Response.Redirect(UnauthorizedPath);
            return;
        }

        // Check roles if specified in metadata
        var authData = endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>();
        var rolesSpecified = authData.Select(a => a.Roles).Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
        if (rolesSpecified.Any())
        {
            var allowed = false;
            foreach (var roleCsv in rolesSpecified)
            {
                var roles = roleCsv.Split(',').Select(r => r.Trim()).Where(r => r.Length > 0);
                foreach (var role in roles)
                {
                    if (user.IsInRole(role))
                    {
                        allowed = true;
                        break;
                    }
                }
                if (allowed) break;
            }

            if (!allowed)
            {
                context.Response.Redirect(UnauthorizedPath);
                return;
            }
        }

        await _next(context);
    }
}

public static class AuthorizationGuardMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthorizationGuard(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthorizationGuardMiddleware>();
    }
}
