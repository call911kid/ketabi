namespace Ketabi.Core.Domain.Entities;

public sealed class BookListingTag : BaseEntity
{
    public BookListingTag() : base() { }
    public BookListingTag(Guid id) : base(id) { }

    public Guid BookListingId { get; set; }
    public string Tag { get; set; } = string.Empty;

    // Navigation
    public BookListing? BookListing { get; set; }
}
