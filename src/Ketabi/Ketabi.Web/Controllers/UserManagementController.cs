using Ketabi.Application.DTOs.Common;
using Ketabi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ketabi.Web.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly IDashboardService _dashboardService;
        public UserManagementController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null)
        {
            var pagination = new PagedRequestDto { Page = page, PageSize = pageSize };
            var users = await _dashboardService.GetUserOverviewAsync(pagination, search);
            ViewData["Search"] = search;
            ViewData["ActiveAdminPage"] = "UserManagement";
            return View(users);
        }
    }
}
