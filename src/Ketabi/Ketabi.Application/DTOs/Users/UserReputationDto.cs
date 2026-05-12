namespace Ketabi.Application.DTOs.Users;

/// <summary>
/// Aggregated stats shown in the navbar profile dropdown.
/// Maps to: NavbarViewModel.
/// </summary>
public class UserReputationDto
{
    public Guid UserId { get; set; }
    public double ReputationScore { get; set; }
    public int ReviewCount { get; set; }
    public int BooksListed { get; set; }
    public int CompletedTransactions { get; set; }
    public int UnreadNotifications { get; set; }
}

// Consolidated DTO for commonly used user statistics. Use this where a compact stats
// payload is required (e.g., navbar, profile summary).
public sealed class UserStatsDto
{
    public double ReputationScore { get; set; }
    public int ReviewCount { get; set; }
    public int BooksListed { get; set; }
    public int CompletedTransactions { get; set; }
    public int UnreadNotifications { get; set; }
}

