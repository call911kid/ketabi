namespace Ketabi.Core.Domain.Entities;

public sealed class User : BaseEntity
{
    public User(Guid id) : base(id) { }
    public User() : base() { }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
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
    public ICollection<Conversation> OwnedConversations { get; set; } = new List<Conversation>();
    public ICollection<Conversation> RequestedConversations { get; set; } = new List<Conversation>();
}
