using Ketabi.Core.Domain.Enums;

namespace Ketabi.Web.ViewModels.Notifications;

// Root ViewModel for Notifications/Index.cshtml — full notifications list.
public class NotificationsIndexViewModel
{
    public IList<NotificationItemViewModel> Notifications { get; set; } = [];
    public int UnreadCount => Notifications.Count(n => !n.IsRead);
    public bool HasUnread => UnreadCount > 0;
}

// Drives a single notification row in the list and navbar dropdown.
public class NotificationItemViewModel
{
    public Guid NotificationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public NotificationType Type { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public string? ActionUrl { get; set; }

    // Bootstrap icon class for the notification type icon
    public string IconClass => Type switch
    {
        NotificationType.RequestUpdate => "bi-envelope-fill text-primary",
        NotificationType.Review => "bi-chat-quote-fill text-info",
        NotificationType.System => "bi-bell-fill text-secondary",
        NotificationType.General => "bi-bell-fill text-secondary",
        NotificationType.Message => "bi-chat-text-fill text-primary",
        _ => "bi-bell"
    };
}
