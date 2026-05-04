using Ketabi.Core.Domain.Enums;
using Ketabi.Application.DTOs.Users;

namespace Ketabi.Application.DTOs.Books;

/// <summary>
/// Complete book record for the Book Detail page.
/// Maps from: UserBook + Category + User entities → BookDetailViewModel.
/// </summary>
public class BookDetailDto
{
    public Guid BookId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string? ISBN { get; init; }
    public string? Description { get; init; }
    public string? Language { get; init; }
    public string? Publisher { get; init; }
    public string Category { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
    public ListingCondition Condition { get; init; }
    public SharingMode SharingMode { get; init; }
    public bool IsAvailable { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string? LocationNote { get; init; }
    public DateTime ListedAt { get; init; }

    /// <summary>Owner summary for the owner card panel.</summary>
    public UserSummaryDto Owner { get; init; } = new();
}
