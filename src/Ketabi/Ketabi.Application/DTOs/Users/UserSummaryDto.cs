namespace Ketabi.Application.DTOs.Users;

/// <summary>
/// Minimal user representation embedded inside Book, Request, Review,
/// Notification, and Navbar response DTOs.
/// Maps from: User entity → UserSummaryViewModel.
/// </summary>
public class UserSummaryDto
{
    public Guid UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string AvatarUrl { get; init; } = string.Empty;

    /// <summary>City + Governorate combined, e.g. "Cairo, Giza".</summary>
    public string Location { get; init; } = string.Empty;

    /// <summary>Aggregate reputation score (0.0 – 5.0).</summary>
    public double ReputationScore { get; init; }
    public int ReviewCount { get; init; }
}
