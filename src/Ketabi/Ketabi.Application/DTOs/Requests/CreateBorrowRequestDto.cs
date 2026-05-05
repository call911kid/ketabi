using System.ComponentModel.DataAnnotations;

namespace Ketabi.Application.DTOs.Requests;

/// <summary>
/// Submit a borrow request for a listing.
/// </summary>
public class CreateBorrowRequestDto
{
    [Required]
    public Guid ListingId { get; init; }

    [Required(ErrorMessage = "Please select a return date.")]
    public DateTime ReturnDate { get; init; }

    [MaxLength(500, ErrorMessage = "Note cannot exceed 500 characters.")]
    public string? Note { get; init; }
}
