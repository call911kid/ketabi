using Ketabi.Core.Domain.Entities;

namespace Ketabi.Core.Interfaces.Repositories;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<IEnumerable<Message>> GetMessagesForConversationAsync(Guid conversationId);
    Task MarkAllAsReadAsync(Guid conversationId, Guid readerId);
    Task<int> CountUnreadAsync(Guid userId);
}
