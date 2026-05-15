using Ketabi.Core.Domain.Enums;

namespace Ketabi.Application.DTOs.Notifications;

public class CreateNotificationDto
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public NotificationType NotificationType { get; set; }
}
