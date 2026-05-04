namespace Ketabi.Application.DTOs.Notifications;

/// <summary>
/// Unread count + recent notifications for the navbar dropdown.
/// Maps to: NavbarViewModel.UnreadNotifications.
/// </summary>
public class NotificationSummaryDto
{
    public int UnreadCount { get; init; }
    public IReadOnlyList<NotificationDto> Recent { get; init; } = [];
}
