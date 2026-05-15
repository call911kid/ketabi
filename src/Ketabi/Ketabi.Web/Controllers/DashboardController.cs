using Ketabi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ketabi.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IBookListingService _bookListingService;
        public DashboardController(IDashboardService dashboardService, IBookListingService bookListingService)
        {
            _dashboardService = dashboardService;
            _bookListingService = bookListingService;
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

        [HttpPost]
        public async Task<IActionResult> ApproveBook(Guid bookId)
        {
            try
            {
                await _bookListingService.ApproveListingAsync(bookId);
                TempData["SuccessMessage"] = "Book approved successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to approve book: " + ex.Message;
            }
            return RedirectToAction("Moderation");
        }

        [HttpPost]
        public async Task<IActionResult> RejectBook(Guid bookId, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["ErrorMessage"] = "Rejection reason is required.";
                return RedirectToAction("Moderation");
            }

            try
            {
                await _bookListingService.RejectListingAsync(bookId, reason);
                TempData["SuccessMessage"] = "Book rejected successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to reject book: " + ex.Message;
            }
            return RedirectToAction("Moderation");
        }
    }
}