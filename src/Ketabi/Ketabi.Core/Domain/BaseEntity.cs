namespace Ketabi.Core.Domain;

public abstract class BaseEntity(Guid id)
{
    public Guid Id { get; init; } = id == Guid.Empty ? Guid.NewGuid() : id;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
