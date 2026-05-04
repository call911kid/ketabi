using System.ComponentModel.DataAnnotations;

namespace Ketabi.Application.DTOs.Requests;

/// <summary>
/// POST /books/{id}/exchange — submit a new exchange request.
/// Maps from: ExchangeRequestFormViewModel → CreateExchangeRequestDto → ExchangeRequest entity.
/// SenderId resolved server-side from authenticated identity.
/// </summary>
public class CreateExchangeRequestDto
{
    [Required]
    public Guid BookId { get; init; }

    [Required(ErrorMessage = "Please select a book to offer.")]
    public Guid OfferedBookId { get; init; }

    [MaxLength(500, ErrorMessage = "Note cannot exceed 500 characters.")]
    public string? Note { get; init; }
}
