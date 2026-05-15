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

        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            var overview = await _dashboardService.GetPlatformOverviewAsync();
            ViewData["ActiveAdminPage"] = "Overview";
            return View(overview);
        }
    }
}