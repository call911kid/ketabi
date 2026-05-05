namespace Ketabi.Core.Domain.Entities;

using Ketabi.Core.Domain.Enums;

public class Request : BaseEntity
{
    public Request() : base() { }
    public Request(Guid id) : base(id) { }

    public RequestType Type { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public string? Note { get; set; }
    public DateTime? ReturnDate { get; set; }

    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public Guid ListingId { get; set; }
    public Guid? OfferedListingId { get; set; }

    // Navigation
    public User? Sender { get; set; }
    public User? Receiver { get; set; }
    public BookListing? Listing { get; set; }
    public BookListing? OfferedListing { get; set; }

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
