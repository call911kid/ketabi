using Ketabi.Application.Common;
using Ketabi.Application.Interfaces;
using Ketabi.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Ketabi.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected IUserService UserService => HttpContext.RequestServices.GetService<IUserService>();
        // Populate NavbarViewModel before each action executes.
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await base.OnActionExecutionAsync(context, next);
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
                    var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (Guid.TryParse(userIdClaim, out Guid userId))
                    {

                        var summary = await UserService.GetUserByIdAsync(userId);

                        navbar.FullName = summary.FullName;
                        navbar.AvatarUrl = summary.AvatarUrl ?? AppConstants.DefaultProfilePic;
                        navbar.ReputationScore = (int)summary.ReputationScore;
                        navbar.ReviewCount = summary.ReviewCount;
                        //navbar.BooksListed = summary.BooksListed;
                        navbar.CompletedTransactions = summary.TradesCount;

                    }
                }
                // Place into ViewData so _Layout can pass it explicitly to the partial
                ViewData["NavbarModel"] = navbar;

                var controller = context?.RouteData?.Values["controller"]?.ToString() ?? string.Empty;
                var action = context?.RouteData?.Values["action"]?.ToString() ?? string.Empty;
                navbar.IsExplorerPage = string.Equals(controller, "Home", StringComparison.OrdinalIgnoreCase)
                                         && string.Equals(action, "Index", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                // Be conservative: never throw from layout population
            }
        }
    }
}
