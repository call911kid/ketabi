namespace Ketabi.Application.DTOs.Users;

/// <summary>
/// Full profile response. Maps from: User entity + aggregated stats
/// → ProfileViewModel.
/// </summary>
public class UserProfileDto
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string? Bio { get; init; }
    public string AvatarUrl { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;

    /// <summary>Human-readable join date, e.g. "Member since January 2024".</summary>
    public string MemberSince { get; init; } = string.Empty;

    // Reputation & Stats
    public double ReputationScore { get; init; }
    public int ReviewCount { get; init; }
    public int BooksListed { get; init; }
    public int ActiveListings { get; init; }
    public int CompletedBorrows { get; init; }
    public int CompletedExchanges { get; init; }

    /// <summary>True when the profile belongs to the currently authenticated user.</summary>
    public bool IsOwnProfile { get; init; }
}
