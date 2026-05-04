using Ketabi.Application.DTOs.Users;

namespace Ketabi.Application.DTOs.Reviews;

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
