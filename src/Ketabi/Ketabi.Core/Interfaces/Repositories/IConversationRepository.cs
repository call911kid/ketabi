using Ketabi.Core.Domain.Entities;

namespace Ketabi.Core.Interfaces.Repositories;

public interface IConversationRepository : IGenericRepository<Conversation>
{
    Task<Conversation?> GetWithDetailsAsync(Guid conversationId);
    Task<IEnumerable<Conversation>> GetConversationsForUserAsync(Guid userId);
    Task<Conversation?> GetByRequestIdAsync(Guid requestId);
    Task<bool> IsParticipantAsync(Guid conversationId, Guid userId);
}
