using Ketabi.Web.ViewModels.Shared;

namespace Ketabi.Web.ViewModels.Profile;

public class ProfileViewModel
{
    // Identity & Bio
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    // Convenience combined values for views
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string AvatarUrl { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    // e.g. "Member since January 2024".
    public string MemberSince { get; set; } = string.Empty;

    // Reputation & Stats
    public double ReputationScore { get; set; }
    public int ReviewCount { get; set; }
    public int BooksListed { get; set; }
    public int ActiveListings { get; set; }
    public int CompletedBorrows { get; set; }
    public int CompletedExchanges { get; set; }

    // Formatted to 1 decimal (e.g. "4.8").
    public string FormattedRating => ReputationScore.ToString("F1");

    public int TotalCompleted => CompletedBorrows + CompletedExchanges;

    // Viewer Context

    // True when the authenticated user is viewing their own profile.
    public bool IsOwnProfile { get; set; }

    // Content Sections

    // Books listed by this user (paginated, 6 per load).
    public IList<BookCardViewModel> Books { get; set; } = [];
    public PagerViewModel BooksPager { get; set; } = new();

    // Reviews received by this user (paginated, 5 per load).
    public IList<ReviewItemViewModel> Reviews { get; set; } = [];
    public PagerViewModel ReviewsPager { get; set; } = new();

    // Edit Modal (pre-populated; only rendered when IsOwnProfile)
    public EditProfileViewModel? EditForm { get; set; }
}
