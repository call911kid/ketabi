using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Interfaces.Repositories;
using Ketabi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ketabi.Infrastructure.Repositories;

internal class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    public MessageRepository(KetabiDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Message>> GetMessagesForConversationAsync(Guid conversationId) =>
        await _dbSet
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .Include(m => m.Sender)
            .ToListAsync();

    public Task<int> CountUnreadAsync(Guid userId) =>
        _dbSet.CountAsync(m =>
            (m.Conversation!.OwnerId == userId || m.Conversation!.RequesterId == userId)
            && m.SenderId != userId
            && !m.IsRead);

    public async Task MarkAllAsReadAsync(Guid conversationId, Guid readerId) =>
        await _dbSet
            .Where(m => m.ConversationId == conversationId
                     && m.SenderId != readerId
                     && !m.IsRead)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(m => m.IsRead, true));
}
