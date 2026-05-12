namespace Ketabi.Application.DTOs.Users;

/// <summary>
/// Full profile response. Maps from: User entity + aggregated stats
/// → ProfileViewModel.
/// </summary>
public class UserProfileDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string AvatarUrl { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    /// <summary>Human-readable join date, e.g. "Member since January 2024".</summary>
    public string MemberSince { get; set; } = string.Empty;

    // Reputation & Stats grouped under UserStatsDto for reusability
    public UserStatsDto Stats { get; set; } = new UserStatsDto();

    /// <summary>True when the profile belongs to the currently authenticated user.</summary>
    public bool IsOwnProfile { get; set; }
}
