using Ketabi.Core.Domain.Enums;
using Ketabi.Web.ViewModels.Shared;

namespace Ketabi.Web.ViewModels.Requests;

// Drives _RequestCard.cshtml partial view.
// Supports both BorrowRequest and ExchangeRequest discriminated by IsBorrow.
public class RequestCardViewModel
{
    public Guid RequestId { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime RequestDate { get; set; }
    public string? Note { get; set; }

    // Book being requested
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string BookImageUrl { get; set; } = string.Empty;
    public string BookCategory { get; set; } = string.Empty;

    // Requester (Sender)
    public UserSummaryViewModel Requester { get; set; } = new();

    // Owner (Receiver)
    public UserSummaryViewModel Owner { get; set; } = new();

    // Type Discriminator — true = BorrowRequest, false = ExchangeRequest
    public bool IsBorrow { get; set; }

    // Expected return date (populated for borrow requests only).
    public DateTime? ReturnDate { get; set; }

    // Offered book (populated for exchange requests only).
    public BookCardViewModel? OfferedBook { get; set; }

    // Display Helpers
    public string TypeLabel => IsBorrow ? "Borrow" : "Exchange";
    public string TypeBadgeCss => IsBorrow ? "badge-borrow" : "badge-exchange";

    public string StatusBadgeCss => Status switch
    {
        RequestStatus.Pending   => "status-pending",
        RequestStatus.Approved  => "status-meetup",
        RequestStatus.Rejected  => "status-rejected",
        RequestStatus.Completed => "status-completed",
        _                       => string.Empty
    };

    public string StatusLabel => Status switch
    {
        RequestStatus.Approved => "Accepted — Arrange Meetup",
        _                      => Status.ToString()
    };

    // Number of days until return. Populated for Accepted borrow requests.
    public int? DaysUntilReturn => ReturnDate.HasValue
        ? (int)(ReturnDate.Value - DateTime.UtcNow).TotalDays
        : null;

    // True when the current user received this request (they own the requested listing).
    public bool IsIncoming { get; set; }

    // Action Visibility Flags

    // Owner can Accept/Reject only when Pending.
    public bool CanAcceptOrReject  { get; set; }

    // Requester can withdraw only when Pending.
    public bool CanWithdraw        { get; set; }

    // The other party shown on the card — requester for received, listing owner for sent.
    public UserSummaryViewModel Counterparty => IsIncoming ? Requester : Owner;

    public string CounterpartyRoleLabel => IsIncoming ? "Requester" : "Owner";

    public string CounterpartySectionLabel => IsIncoming ? "From" : "To";

    // Populated when Status == Approved and a Conversation exists
    public Guid? ConversationId { get; set; }
    public bool HasConversation => ConversationId.HasValue;
}
