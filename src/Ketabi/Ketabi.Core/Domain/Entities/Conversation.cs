namespace Ketabi.Core.Domain.Entities;

public class Conversation : BaseEntity
{
    public Conversation() : base() { }
    public Conversation(Guid id) : base(id) { }

    public bool RequesterConfirmedHandoff { get; set; }
    public bool OwnerConfirmedHandoff { get; set; }

    // Navigation properties
    public Guid OwnerId { get; set; }
    public User? Owner { get; set; }

    public Guid RequesterId { get; set; }
    public User? Requester { get; set; }

    public Guid RequestId { get; set; }
    public Request? Request { get; set; }

    public ICollection<Message> Messages { get; set; } = new List<Message>();

}
