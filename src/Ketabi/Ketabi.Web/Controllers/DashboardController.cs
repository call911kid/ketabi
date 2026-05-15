using Ketabi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ketabi.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("/Admin")]
        public async Task<IActionResult> Overview()
        {
            var overview = await _dashboardService.GetPlatformOverviewAsync();
            ViewData["ActiveAdminPage"] = "Overview";
            return View(overview);
        }

        [HttpGet]
        public async Task<IActionResult> Moderation()
        {
            var moderation = await _dashboardService.GetBookModerationAsync();
            ViewData["ActiveAdminPage"] = "Moderation";
            return View(moderation);
        }
    }
}