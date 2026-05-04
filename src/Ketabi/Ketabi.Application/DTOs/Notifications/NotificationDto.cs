using Ketabi.Core.Domain.Enums;

namespace Ketabi.Application.DTOs.Notifications;

/// <summary>
/// Single notification record returned by the service layer.
/// Maps from: Notification entity → NotificationItemViewModel.
/// </summary>
public class NotificationDto
{
    public Guid NotificationId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public bool IsRead { get; init; }
    public NotificationType Type { get; init; }
    public DateTime CreatedAt { get; init; }

    /// <summary>Human-readable relative time, e.g. "5 minutes ago". Computed by the service.</summary>
    public string TimeAgo { get; init; } = string.Empty;

    /// <summary>Optional deep-link URL to the related entity (book, request, etc.).</summary>
    public string? ActionUrl { get; init; }
}
