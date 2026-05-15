using Ketabi.Core.Domain.Enums;

namespace Ketabi.Web.ViewModels.Requests;

public class RequestsIndexViewModel
{
    public IList<RequestCardViewModel> IncomingRequests { get; set; } = [];
    public IList<RequestCardViewModel> OutgoingRequests { get; set; } = [];
    public IList<PendingBookCardViewModel> PendingBooks { get; set; } = [];

    // "incoming" | "outgoing" | "pending". Drives the active Bootstrap tab.
    public string ActiveTab { get; set; } = "incoming";

    public bool IsIncomingEmpty => !IncomingRequests.Any();
    public bool IsOutgoingEmpty => !OutgoingRequests.Any();
    public bool IsPendingEmpty => !PendingBooks.Any();

    // Counts for tab badges
    public int IncomingCount => IncomingRequests.Count;
    public int OutgoingCount => OutgoingRequests.Count;
    public int PendingCount => PendingBooks.Count;

    public int PendingIncomingCount =>
        IncomingRequests.Count(r => r.Status == RequestStatus.Pending);
}
