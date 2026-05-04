namespace Ketabi.Core.Domain.Entities;

using Ketabi.Core.Domain.Enums;

public abstract class Request : BaseEntity
{
    protected Request() : base() { }
    protected Request(Guid id) : base(id) { }

    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public string? Note { get; set; }

    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public Guid ListingId { get; set; }

    // Navigation
    public User? Sender { get; set; }
    public User? Receiver { get; set; }
    public BookListing? Listing { get; set; }

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}