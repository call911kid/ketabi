namespace Ketabi.Application.DTOs.Requests;

/// <summary>
/// Admin view of trade requests for moderation.
/// </summary>
public class AdminTradeRequestDto
{
    public string Id { get; init; } = string.Empty;
    public string BookTitle { get; init; } = string.Empty;
    public string BookCover { get; init; } = string.Empty;
    public string RequesterName { get; init; } = string.Empty;
    public string OwnerName { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string CreatedAt { get; init; } = string.Empty;
    public int? DurationDays { get; init; }
}