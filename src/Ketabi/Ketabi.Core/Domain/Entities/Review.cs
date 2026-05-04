namespace Ketabi.Core.Domain.Entities;

public sealed class Review : BaseEntity
{
    public Review(Guid id) : base(id) { }
    public Review() : base() { }

    public int Rating { get; set; }
    public string? Comment { get; set; }

    public Guid TargetUserId { get; set; }
    public Guid ReviewerId { get; set; }
    public Guid? RelatedRequestId { get; set; }

    // Navigation
    public User? TargetUser { get; set; }
    public User? Reviewer { get; set; }
    public Request? RelatedRequest { get; set; }
}
