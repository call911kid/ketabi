using Ketabi.Application.DTOs.Common;
using Ketabi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ketabi.Web.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IAuthService _authService;

        public UserManagementController(IDashboardService dashboardService, IAuthService authService)
        {
            _dashboardService = dashboardService;
            _authService = authService;
        }
        [HttpGet("/Admin/user-management")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null)
        {
            var pagination = new PagedRequestDto { Page = page, PageSize = pageSize };
            var users = await _dashboardService.GetUserOverviewAsync(pagination, search);
            ViewData["Search"] = search;
            ViewData["ActiveAdminPage"] = "UserManagement";
            return View(users);
        }

        [HttpPost("/Admin/user-management/add-role")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole([FromBody] AddRoleRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Role))
                return BadRequest(new { error = "Email and Role are required" });

            await _authService.AddToRoleAsync(request.Email, request.Role);
            return Ok(new { success = true });
        }

        public class AddRoleRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }
    }
}
