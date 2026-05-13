namespace Ketabi.Application.DTOs.Users;

/// <summary>
/// Admin-specific user information for user management dashboard.
/// </summary>
public class AdminUserDto
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string MemberSince { get; init; } = string.Empty;
    public int BooksListed { get; init; }
    public int CompletedTransactions { get; init; }
    public string Location { get; init; } = string.Empty;
    public string LastActive { get; init; } = string.Empty;
    public int ReportCount { get; init; }
}