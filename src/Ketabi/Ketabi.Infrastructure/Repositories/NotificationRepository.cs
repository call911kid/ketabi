using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Interfaces.Repositories;
using Ketabi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ketabi.Infrastructure.Repositories;

internal class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(KetabiDbContext context) : base(context) { }

    public async Task<IEnumerable<Notification>> GetNotificationsForUserAsync(Guid userId, int page, int pageSize)
        => await _dbSet
            .Where(n => n.UserId == userId && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public async Task<int> GetUnreadCountAsync(Guid userId)
        => await _dbSet.CountAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted);

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var unread = await _dbSet
            .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
            .ToListAsync();

        foreach (var n in unread)
        {
            n.IsRead = true;
            n.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetTotalCountAsync(Guid userId)
        => await _dbSet.CountAsync(n => n.UserId == userId && !n.IsDeleted);
}
