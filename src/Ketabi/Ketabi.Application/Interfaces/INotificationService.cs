using Ketabi.Application.DTOs.Notifications;

namespace Ketabi.Application.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetNotificationsAsync(Guid userId, int page, int pageSize);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task MarkAllAsReadAsync(Guid userId);
    Task MarkAsReadAsync(Guid userId, Guid notificationId);
    Task CreateNotificationAsync(CreateNotificationDto dto);
    Task<int> GetTotalCountAsync(Guid userId);
}
