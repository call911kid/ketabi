using System.ComponentModel.DataAnnotations;
using Ketabi.Core.Domain.Enums;

namespace Ketabi.Application.DTOs.Books;

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
    [Range(1, 365, ErrorMessage = "Sharing duration must be between 1 and 365 days.")]
    public int? SharingDurationInDays { get; init; }
    

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
