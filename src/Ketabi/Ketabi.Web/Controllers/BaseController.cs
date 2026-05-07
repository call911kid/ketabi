using Ketabi.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Ketabi.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        // Populate NavbarViewModel before each action executes.
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            // Skip navbar population for certain controllers (e.g., Account) so auth pages
            // don't receive or attempt to render the shared navbar. This allows controllers
            // to continue inheriting BaseController but opt-out by controller name.
            var routeController = context?.RouteData?.Values["controller"]?.ToString() ?? string.Empty;
            if (string.Equals(routeController, "Account", StringComparison.OrdinalIgnoreCase))
            {
                ViewData["NavbarModel"] = null;
                return;
            }

            var navbar = new NavbarViewModel();

            try
            {
                var user = HttpContext?.User;
                navbar.IsAuthenticated = user?.Identity?.IsAuthenticated ?? false;

                if (navbar.IsAuthenticated)
                {
                    // Claims-based population with null-conditional fallbacks
                    navbar.FullName = user?.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
                    navbar.UserName = user?.FindFirst("username")?.Value
                                      ?? user?.FindFirst("preferred_username")?.Value
                                      ?? user?.Identity?.Name
                                      ?? string.Empty;

                    var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (Guid.TryParse(idClaim, out var parsedId))
                    {
                        navbar.CurrentUserId = parsedId;
                    }

                    // Avatar if provided in claims (optional)
                    navbar.AvatarUrl = user?.FindFirst("AvatarUrl")?.Value ?? string.Empty;

                    // Placeholder stats; replace with real service calls when available
                    navbar.ReputationScore = int.Parse(user?.FindFirst("ReputationScore")?.Value ?? "0");
                    navbar.ReviewCount = int.Parse(user?.FindFirst("ReviewCount")?.Value ?? "0");
                    navbar.BooksListed = int.Parse(user?.FindFirst("BooksListed")?.Value ?? "0");
                    navbar.CompletedTransactions = int.Parse(user?.FindFirst("CompletedTransactions")?.Value ?? "0");
                    navbar.UnreadNotifications = int.Parse(user?.FindFirst("UnreadNotifications")?.Value ?? "0");
                }

                var controller = context?.RouteData?.Values["controller"]?.ToString() ?? string.Empty;
                var action = context?.RouteData?.Values["action"]?.ToString() ?? string.Empty;
                navbar.IsExplorerPage = string.Equals(controller, "Home", StringComparison.OrdinalIgnoreCase)
                                         && string.Equals(action, "Index", StringComparison.OrdinalIgnoreCase);

                // Place into ViewData so _Layout can pass it explicitly to the partial
                ViewData["NavbarModel"] = navbar;
            }
            catch
            {
                // Be conservative: never throw from layout population
            }
        }
    }
}
