using System.ComponentModel.DataAnnotations;
using Ketabi.Core.Domain.Enums;

namespace Ketabi.Application.DTOs.Books;

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
    [MaxLength(5, ErrorMessage = "A maximum of 5 tags is allowed.")]
    public List<string>? Tags { get; init; }

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
    [Range(1, 365, ErrorMessage = "Sharing duration must be between 1 and 365 days.")]
    public int? SharingDurationInDays { get; init; }

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
