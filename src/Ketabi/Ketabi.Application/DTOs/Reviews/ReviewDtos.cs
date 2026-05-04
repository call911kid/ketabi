using System.ComponentModel.DataAnnotations;
using Ketabi.Application.DTOs.Books;
using Ketabi.Application.DTOs.Users;

namespace Ketabi.Application.DTOs.Reviews;

// ── Review Response ────────────────────────────────────────────────────────

/// <summary>
/// Review record returned by the service layer.
/// Maps to: ReviewItemViewModel on the profile page.
/// </summary>
public class ReviewDto
{
    public Guid ReviewId { get; init; }

    /// <summary>User who wrote the review.</summary>
    public UserSummaryDto Reviewer { get; init; } = new();

    /// <summary>User who received the review.</summary>
    public UserSummaryDto Reviewee { get; init; } = new();

    /// <summary>Star rating from 1 to 5.</summary>
    public int Rating { get; init; }
    public string? Comment { get; init; }

    /// <summary>Title of the book the transaction was about.</summary>
    public string RelatedBookTitle { get; init; } = string.Empty;

    public Guid? RelatedRequestId { get; init; }
    public DateTime CreatedAt { get; init; }

    /// <summary>Human-readable relative time, e.g. "2 days ago". Computed by the service.</summary>
    public string TimeAgo { get; init; } = string.Empty;
}

// ── Create Review ──────────────────────────────────────────────────────────

/// <summary>
/// POST /reviews — submit a review after a completed transaction.
/// ReviewerId resolved server-side from authenticated identity.
/// </summary>
public class CreateReviewDto
{
    [Required]
    public Guid RevieweeId { get; init; }

    [Required]
    public Guid RelatedRequestId { get; init; }

    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; init; }

    [MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
    public string? Comment { get; init; }
}
