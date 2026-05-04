using Ketabi.Core.Domain.Enums;
using Ketabi.Application.DTOs.Books;
using Ketabi.Application.DTOs.Users;

namespace Ketabi.Application.DTOs.Requests;

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
