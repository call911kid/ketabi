namespace Ketabi.Core.Domain.Entities;

using Ketabi.Core.Domain.Enums;

public sealed class BookListing : BaseEntity
{
    public BookListing() : base() { }
    public BookListing(Guid id) : base(id) { }

    public required string Title { get; set; }
    public required string Author { get; set; }
    public string? ISBN { get; set; }
    public string? Description { get; set; }
    public string? Language { get; set; }
    public string? Publisher { get; set; }
    public ListingCondition Condition { get; set; }
    public SharingMode SharingMode { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? ImageUrl { get; set; }
    public string? LocationNote { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UserId { get; set; }

    // Navigation
    public User? User { get; set; }
    public Category? Category { get; set; }
    public ICollection<Request> Requests { get; set; } = new List<Request>();
}
