namespace Ketabi.Application.DTOs.Users;

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
