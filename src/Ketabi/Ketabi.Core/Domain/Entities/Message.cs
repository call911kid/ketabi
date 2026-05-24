namespace Ketabi.Core.Domain.Entities;

public class Message : BaseEntity
{
    public Message() : base() { }

    public Message(Guid id) : base(id) { }

    public string Text { get; set; } = string.Empty;
    public bool IsRead { get; set; }

    // Navigation properties
    public Guid ConversationId { get; set; }
    public Conversation? Conversation { get; set; }

    public Guid SenderId { get; set; }
    public User? Sender { get; set; }

}
