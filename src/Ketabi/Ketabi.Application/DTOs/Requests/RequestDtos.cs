using System.ComponentModel.DataAnnotations;
using Ketabi.Core.Domain.Enums;
using Ketabi.Application.DTOs.Books;
using Ketabi.Application.DTOs.Users;

namespace Ketabi.Application.DTOs.Requests;

// ── Book Request Response ──────────────────────────────────────────────────

/// <summary>
/// Full request record returned by GET /requests and GET /requests/{id}.
/// Discriminated by IsBorrow: true = BorrowRequest, false = ExchangeRequest.
/// Maps to: RequestCardViewModel.
/// </summary>
public class BookRequestDto
{
    public Guid RequestId { get; init; }
    public RequestStatus Status { get; init; }
    public DateTime RequestDate { get; init; }
    public string? Note { get; init; }

    /// <summary>Book that was requested.</summary>
    public BookSummaryDto Book { get; init; } = new();

    /// <summary>User who submitted the request (Sender).</summary>
    public UserSummaryDto Requester { get; init; } = new();

    /// <summary>User who owns the book (Receiver).</summary>
    public UserSummaryDto Owner { get; init; } = new();

    /// <summary>True = BorrowRequest. False = ExchangeRequest.</summary>
    public bool IsBorrow { get; init; }

    /// <summary>Expected return date. Populated only for borrow requests.</summary>
    public DateTime? ReturnDate { get; init; }

    /// <summary>Book offered in exchange. Populated only for exchange requests.</summary>
    public BookSummaryDto? OfferedBook { get; init; }
}

// ── Create Borrow Request ──────────────────────────────────────────────────

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

// ── Create Exchange Request ────────────────────────────────────────────────

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

// ── Update Request Status ──────────────────────────────────────────────────

/// <summary>
/// PATCH /requests/{id}/status — called by the book owner to approve or reject.
/// Maps from: RequestStatusActionViewModel → UpdateRequestStatusDto → Request entity.
/// </summary>
public class UpdateRequestStatusDto
{
    [Required]
    [EnumDataType(typeof(RequestStatus))]
    public RequestStatus Status { get; init; }

    [MaxLength(400, ErrorMessage = "Note cannot exceed 400 characters.")]
    public string? Note { get; init; }
}
