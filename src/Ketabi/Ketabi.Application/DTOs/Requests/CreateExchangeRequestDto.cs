using System.ComponentModel.DataAnnotations;

namespace Ketabi.Application.DTOs.Requests;

/// <summary>
/// Submit an exchange request for a listing.
/// </summary>
public class CreateExchangeRequestDto
{
    [Required]
    public Guid ListingId { get; init; }

    [Required(ErrorMessage = "Please select a listing to offer.")]
    public Guid OfferedListingId { get; init; }

    [MaxLength(500, ErrorMessage = "Note cannot exceed 500 characters.")]
    public string? Note { get; init; }
}
