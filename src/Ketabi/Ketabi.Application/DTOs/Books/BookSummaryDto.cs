using Ketabi.Core.Domain.Enums;
using Ketabi.Application.DTOs.Users;

namespace Ketabi.Application.DTOs.Books;

/// <summary>
/// Lightweight book representation for Explorer grid cards, profile book grids,
/// request rows, and exchange selectors.
/// Maps from: UserBook + Category + User entities → BookCardViewModel.
/// </summary>
public class BookSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public  string? Description { get; set; }
    public ListingCondition Condition { get; set; }
    public string SharingMode { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string? OwnerImageUrl { get; set; }
    public string? OwnerAvatarUrl { get; set; }
    public double OwnerRating { get; set; }
    public double OwnerReputation { get; set; }
    public double DistanceInKm { get; set; }
}