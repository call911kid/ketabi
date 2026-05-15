namespace Ketabi.Application.DTOs.Users;

/// <summary>
/// Minimal user representation embedded inside Book, Request, Review,
/// Notification, and Navbar response DTOs.
/// Maps from: User entity → UserSummaryViewModel.
/// </summary>
public class UserSummaryDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    // Basic stats — kept minimal for embedding in other DTOs
    public double ReputationScore { get; set; }
    public int ReviewCount { get; set; }
    public int TradesCount { get; set; }
    public int ListingCount { get; set; }
}
