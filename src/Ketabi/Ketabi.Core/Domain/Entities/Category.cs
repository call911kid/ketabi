namespace Ketabi.Core.Domain.Entities;

public sealed class Category : BaseEntity
{
    public Category(Guid id) : base(id) { }

    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? IconUrl { get; set; }

    public ICollection<UserBook> Books { get; set; } = new List<UserBook>();
}
