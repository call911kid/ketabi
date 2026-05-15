using Ketabi.Application.DTOs.Notifications;

namespace Ketabi.Application.Interfaces;

public interface INotificationDispatcher
{
    // Best-effort: must never throw. DB save has already succeeded before this is called.
    Task DispatchAsync(Guid userId, NotificationDto dto);
}
