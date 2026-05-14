namespace Ketabi.Core.Domain.Entities;

public sealed class Category : BaseEntity
{
    public Category(Guid id) : base(id) { }
    public Category() : base() { }

    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public string Emoji { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;

    public ICollection<BookListing> BookListings { get; set; } = new List<BookListing>();
}
