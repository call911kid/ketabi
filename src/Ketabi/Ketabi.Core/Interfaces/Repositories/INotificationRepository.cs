using Ketabi.Core.Domain.Entities;

namespace Ketabi.Core.Interfaces.Repositories;

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<IEnumerable<Notification>> GetNotificationsForUserAsync(Guid userId, int page, int pageSize);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task MarkAllAsReadAsync(Guid userId);
    Task<int> GetTotalCountAsync(Guid userId);
}
