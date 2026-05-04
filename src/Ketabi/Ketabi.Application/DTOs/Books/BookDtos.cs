using System.ComponentModel.DataAnnotations;
using Ketabi.Core.Domain.Enums;
using Ketabi.Application.DTOs.Users;

namespace Ketabi.Application.DTOs.Books;

// ── Book Summary ───────────────────────────────────────────────────────────

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

// ── Book Detail ────────────────────────────────────────────────────────────

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

// ── Active Request Info ────────────────────────────────────────────────────

/// <summary>
/// Viewer's active request state on a book. Embedded inside BookDetailDto
/// to drive the request action panel in BookDetailViewModel.
/// </summary>
public class ActiveRequestInfoDto
{
    /// <summary>True when the authenticated viewer has an active request on this book.</summary>
    public bool HasActiveRequest { get; init; }
    public RequestStatus? ViewerRequestStatus { get; init; }

    /// <summary>True when the authenticated viewer owns the book.</summary>
    public bool IsOwner { get; init; }
}

// ── Create Book ────────────────────────────────────────────────────────────

/// <summary>
/// POST /books — list a new book.
/// Maps from: CreateBookViewModel → CreateBookDto → UserBook entity.
/// OwnerId is resolved server-side from the authenticated user's identity.
/// </summary>
public class CreateBookDto
{
    [Required(ErrorMessage = "Title is required.")]
    [MinLength(1)]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string Title { get; init; } = string.Empty;

    [Required(ErrorMessage = "Author is required.")]
    [MaxLength(200, ErrorMessage = "Author cannot exceed 200 characters.")]
    public string Author { get; init; } = string.Empty;

    [MaxLength(13, ErrorMessage = "ISBN cannot exceed 13 characters.")]
    public string? ISBN { get; init; }

    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    public string? Description { get; init; }

    [MaxLength(60)]
    public string? Language { get; init; }

    [MaxLength(100)]
    public string? Publisher { get; init; }

    [Required(ErrorMessage = "Category is required.")]
    public Guid CategoryId { get; init; }

    [Required(ErrorMessage = "Condition is required.")]
    [EnumDataType(typeof(ListingCondition))]
    public ListingCondition Condition { get; init; }

    [Required(ErrorMessage = "Sharing mode is required.")]
    [EnumDataType(typeof(SharingMode))]
    public SharingMode SharingMode { get; init; }

    /// <summary>
    /// Resolved URL of the uploaded cover image.
    /// The Web layer resolves IFormFile → URL before populating this field.
    /// </summary>
    [Required(ErrorMessage = "Cover image is required.")]
    [Url]
    [MaxLength(500)]
    public string ImageUrl { get; init; } = string.Empty;

    [MaxLength(120)]
    public string? LocationNote { get; init; }
}

// ── Update Book ────────────────────────────────────────────────────────────

/// <summary>
/// PATCH /books/{id} — update an existing listing.
/// Maps from: EditBookViewModel → UpdateBookDto → UserBook entity.
/// All fields are optional; null fields are not updated.
/// </summary>
public class UpdateBookDto
{
    [MinLength(1)]
    [MaxLength(200)]
    public string? Title { get; init; }

    [MaxLength(200)]
    public string? Author { get; init; }

    [MaxLength(13)]
    public string? ISBN { get; init; }

    [MaxLength(2000)]
    public string? Description { get; init; }

    [MaxLength(60)]
    public string? Language { get; init; }

    [MaxLength(100)]
    public string? Publisher { get; init; }

    public Guid? CategoryId { get; init; }

    [EnumDataType(typeof(ListingCondition))]
    public ListingCondition? Condition { get; init; }

    [EnumDataType(typeof(SharingMode))]
    public SharingMode? SharingMode { get; init; }

    /// <summary>
    /// Resolved URL of the replacement cover image.
    /// The Web layer resolves IFormFile? → URL before populating this field.
    /// Null means keep the current image.
    /// </summary>
    [Url]
    [MaxLength(500)]
    public string? ImageUrl { get; init; }

    [MaxLength(120)]
    public string? LocationNote { get; init; }

    public bool? IsAvailable { get; init; }
}
