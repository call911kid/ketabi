using System.ComponentModel.DataAnnotations;

namespace Ketabi.Application.DTOs.Reviews;

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
