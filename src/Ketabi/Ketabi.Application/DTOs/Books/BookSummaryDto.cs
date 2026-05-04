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
    public Guid BookId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;

    /// <summary>Category display name resolved from the Category navigation.</summary>
    public string Category { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }

    public ListingCondition Condition { get; init; }
    public SharingMode SharingMode { get; init; }
    public bool IsAvailable { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string LocationNote { get; init; } = string.Empty;

    /// <summary>Owner summary embedded for card rendering.</summary>
    public UserSummaryDto Owner { get; init; } = new();
}
