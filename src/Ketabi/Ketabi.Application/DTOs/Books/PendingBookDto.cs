namespace Ketabi.Application.DTOs.Books;

/// <summary>
/// Represents a book listing pending admin approval.
/// </summary>
public class PendingBookDto
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Condition { get; init; } = string.Empty;
    public string OwnerId { get; init; } = string.Empty;
    public string OwnerName { get; init; } = string.Empty;
    public string SubmittedAt { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? RejectionReason { get; init; }
    public string Description { get; init; } = string.Empty;
    public string TransactionType { get; init; } = string.Empty;
    public string CoverColor { get; init; } = string.Empty;
}