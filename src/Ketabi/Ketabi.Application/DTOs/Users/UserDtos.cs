using System.ComponentModel.DataAnnotations;

namespace Ketabi.Application.DTOs.Users;

// ── User Summary ───────────────────────────────────────────────────────────

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

// ── User Profile ───────────────────────────────────────────────────────────

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

// ── Update Profile ─────────────────────────────────────────────────────────

/// <summary>
/// PATCH /profile/edit — partial update of the authenticated user's profile.
/// Maps from: EditProfileViewModel → UpdateUserProfileDto → User entity.
/// All fields are optional; null fields are not updated.
/// </summary>
public class UpdateUserProfileDto
{
    [MinLength(2, ErrorMessage = "Full name must be at least 2 characters.")]
    [MaxLength(80, ErrorMessage = "Full name cannot exceed 80 characters.")]
    public string? FullName { get; init; }

    [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
    [MaxLength(30, ErrorMessage = "Username cannot exceed 30 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$",
        ErrorMessage = "Username may only contain letters, numbers, and underscores.")]
    public string? UserName { get; init; }

    [MaxLength(200, ErrorMessage = "Bio cannot exceed 200 characters.")]
    public string? Bio { get; init; }

    [MaxLength(60)]
    public string? City { get; init; }

    [MaxLength(60)]
    public string? Governorate { get; init; }

    /// <summary>
    /// Resolved URL of the uploaded profile picture.
    /// The Web layer resolves IFormFile → URL before populating this field.
    /// </summary>
    [Url]
    [MaxLength(500)]
    public string? ProfilePictureUrl { get; init; }
}

// ── User Reputation ────────────────────────────────────────────────────────

/// <summary>
/// Aggregated stats shown in the navbar profile dropdown.
/// Maps to: NavbarViewModel.
/// </summary>
public class UserReputationDto
{
    public Guid UserId { get; init; }
    public double ReputationScore { get; init; }
    public int ReviewCount { get; init; }
    public int BooksListed { get; init; }
    public int CompletedTransactions { get; init; }
    public int UnreadNotifications { get; init; }
}
