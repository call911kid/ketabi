namespace Ketabi.Core.Domain.Entities;

using Ketabi.Core.Domain.Enums;

public sealed class Notification : BaseEntity
{
    public Notification(Guid id) : base(id) { }
    public Notification() : base() { }

    public required string Title { get; set; }
    public required string Content { get; set; }
    public bool IsRead { get; set; }
    public Guid UserId { get; set; }
    public NotificationType NotificationType { get; set; }

    // Navigation
    public User? User { get; set; }
}
