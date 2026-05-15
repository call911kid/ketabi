using Ketabi.Application.Interfaces;
using Ketabi.Core.Domain.Enums;
using Ketabi.Web.ViewModels.Notifications;
using Ketabi.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ketabi.Web.Controllers;

[Authorize]
public class NotificationsController : BaseController
{
    private const int PageSize = 15;
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    // GET /Notifications
    public async Task<IActionResult> Index(int page = 1)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return RedirectToAction("Login", "Account");

        var notifications = await _notificationService.GetNotificationsAsync(userId, page, PageSize);
        var unreadCount   = await _notificationService.GetUnreadCountAsync(userId);
        var totalCount    = await _notificationService.GetTotalCountAsync(userId);
        int totalPages    = (int)Math.Ceiling(totalCount / (double)PageSize);

        var vm = new NotificationsViewModel
        {
            UnreadCount = unreadCount,
            TotalCount  = totalCount,
            Pager = new PagerViewModel
            {
                CurrentPage = page,
                TotalPages  = totalPages,
                TotalCount  = totalCount
            },
            Notifications = notifications.Select(n => new NotificationItemViewModel
            {
                NotificationId = n.NotificationId,
                Title = n.Title,
                Content = n.Content,
                IsRead = n.IsRead,
                NotificationType = n.NotificationType,
                TimeAgo = n.TimeAgo,
                TypeIcon = GetIcon(n.NotificationType),
                TypeIconColor = GetIconColor(n.NotificationType),
                TypeIconBg = GetIconBg(n.NotificationType),
            }).ToList()
        };

        ViewData["Title"] = "Notifications";
        return View(vm);
    }

    // POST /Notifications/MarkAllRead
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllRead()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdString, out var userId))
            await _notificationService.MarkAllAsReadAsync(userId);

        return RedirectToAction(nameof(Index));
    }

    // POST /Notifications/MarkRead 
    [HttpPost]
    public async Task<IActionResult> MarkRead([FromBody] Guid notificationId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        await _notificationService.MarkAsReadAsync(userId, notificationId);
        return Ok();
    }

    // ── Icon mapping helpers ──────────────────────────────────────────────
    private static string GetIcon(NotificationType t) => t switch
    {
        NotificationType.RequestUpdate => "bi-arrow-left-right",
        NotificationType.Review => "bi-star-fill",
        NotificationType.Message => "bi-chat-dots",
        NotificationType.System => "bi-info-circle",
        _ => "bi-bell"
    };

    private static string GetIconColor(NotificationType t) => t switch
    {
        NotificationType.RequestUpdate => "var(--color-indigo)",
        NotificationType.Review => "var(--color-warning-star)",
        NotificationType.Message => "var(--color-info)",
        NotificationType.System => "var(--color-text-muted)",
        _ => "var(--color-indigo)"
    };

    private static string GetIconBg(NotificationType t) => t switch
    {
        NotificationType.RequestUpdate => "var(--color-indigo-light)",
        NotificationType.Review => "#FFF7ED",
        NotificationType.Message => "var(--color-info-bg)",
        NotificationType.System => "var(--color-surface-muted)",
        _ => "var(--color-indigo-light)"
    };
}
