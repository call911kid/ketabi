using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Interfaces.Repositories;
using Ketabi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ketabi.Infrastructure.Repositories;

internal class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
{
    public ConversationRepository(KetabiDbContext context) : base(context)
    {
    }
    public async Task<IEnumerable<Conversation>> GetConversationsForUserAsync(Guid userId) =>
         await _dbSet.Include(c => c.Owner)
            .Include(c => c.Requester)
            .Include(c => c.Request)
                .ThenInclude(r => r.Listing)
            .Include(c => c.Messages
                .OrderByDescending(m => m.CreatedAt)
                .Take(1))
                .ThenInclude(m => m.Sender)
            .Where(r => r.OwnerId == userId || r.RequesterId == userId)
            .ToListAsync();

    public Task<Conversation?> GetWithDetailsAsync(Guid conversationId) =>
        _dbSet.Include(c => c.Owner)
            .Include(c => c.Requester)
            .Include(c => c.Request)
                .ThenInclude(r => r.Listing)
            .Include(c => c.Messages.OrderBy(m => m.CreatedAt))
                .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

    public Task<Conversation?> GetByRequestIdAsync(Guid requestId) =>
    _dbSet.FirstOrDefaultAsync(c => c.RequestId == requestId);
    public Task<bool> IsParticipantAsync(Guid conversationId, Guid userId) =>
        _dbSet.AnyAsync(c => c.Id == conversationId
            && (c.OwnerId == userId || c.RequesterId == userId));
}
