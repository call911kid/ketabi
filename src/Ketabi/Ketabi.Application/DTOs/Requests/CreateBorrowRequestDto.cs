using System.ComponentModel.DataAnnotations;

namespace Ketabi.Application.DTOs.Requests;

/// <summary>
/// POST /books/{id}/borrow — submit a new borrow request.
/// Maps from: BorrowRequestFormViewModel → CreateBorrowRequestDto → BorrowRequest entity.
/// SenderId resolved server-side from authenticated identity.
/// </summary>
public class CreateBorrowRequestDto
{
    [Required]
    public Guid BookId { get; init; }

    [Required(ErrorMessage = "Please select a return date.")]
    public DateTime ReturnDate { get; init; }

    [MaxLength(500, ErrorMessage = "Note cannot exceed 500 characters.")]
    public string? Note { get; init; }
}
