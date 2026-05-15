using Ketabi.Core.Domain.Enums;
using Ketabi.Web.ViewModels.Shared;

namespace Ketabi.Web.ViewModels.Notifications;

public class NotificationsViewModel
{
    public IList<NotificationItemViewModel> Notifications { get; set; } = [];
    public int UnreadCount { get; set; }
    public int TotalCount { get; set; }
    public PagerViewModel Pager { get; set; } = new();
}

public class NotificationItemViewModel
{
    public Guid NotificationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public NotificationType NotificationType { get; set; }
    public string TimeAgo { get; set; } = string.Empty;

    // Computed icon/color from NotificationType — set in controller
    public string TypeIcon { get; set; } = string.Empty;
    public string TypeIconColor { get; set; } = string.Empty;
    public string TypeIconBg { get; set; } = string.Empty;
}
