namespace Ketabi.Core.Domain.Entities;

public sealed class User : BaseEntity
{
    public User(Guid id) : base(id) { }

    public required string UserName { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? Governorate { get; set; }
    public double ReputationScore { get; set; }
    public string? ProfilePictureUrl { get; set; }

    // Navigation
    public ICollection<BookListing> Listings { get; set; } = new List<BookListing>();
    public ICollection<Request> SentRequests { get; set; } = new List<Request>();
    public ICollection<Request> ReceivedRequests { get; set; } = new List<Request>();
    public ICollection<Review> ReviewsWritten { get; set; } = new List<Review>();
    public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
