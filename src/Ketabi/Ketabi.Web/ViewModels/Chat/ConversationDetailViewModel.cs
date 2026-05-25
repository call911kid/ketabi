namespace Ketabi.Web.ViewModels.Chat;

public class ConversationDetailViewModel
{
    public string ConversationId { get; set; } = string.Empty;
    public BookSummaryViewModel Book { get; set; } = new();
    public OtherUserViewModel OtherUser { get; set; } = new();
    public TransactionStatus TransactionStatus { get; set; }
    public RequestType RequestType { get; set; }

    /// <summary>Duration in days — only relevant for Borrow requests.</summary>
    public int? BorrowDurationDays { get; set; }

    public IList<MessageViewModel> Messages { get; set; } = [];

    /// <summary>Underlying request id for this conversation (used to change request status).</summary>
    public string RequestId { get; set; } = string.Empty;

    /// <summary>True when the current user has already confirmed the handoff.</summary>
    public bool CurrentUserConfirmedHandoff { get; set; }

    /// <summary>True when the other party has already confirmed the handoff.</summary>
    public bool OtherUserConfirmedHandoff { get; set; }

    /// <summary>True when the current user has already submitted a review.</summary>
    public bool ReviewAlreadySubmitted { get; set; }

    /// <summary>
    /// Avatar URL (filename only, e.g. "abc.jpg") for the currently authenticated user.
    /// Populated by ChatController from the appropriate party slot (owner or requester).
    /// Used in the handoff bar "You" participant slot.
    /// </summary>
    public string CurrentUserAvatarUrl { get; set; } = string.Empty;

    // ── Computed helpers (used in views) ────────────────────────────

    /// <summary>
    /// BUG 2 FIX: Show the handoff bar only when one party has already confirmed
    /// but the current user has NOT yet confirmed. Excludes Active (no-one confirmed)
    /// and Completed (both confirmed).
    /// </summary>
    public bool ShowHandoffBar =>
        !CurrentUserConfirmedHandoff &&
        TransactionStatus != TransactionStatus.Active &&
        TransactionStatus != TransactionStatus.Completed;

    public bool ShowReviewForm =>
        TransactionStatus == TransactionStatus.Completed && !ReviewAlreadySubmitted;

    public bool ShowMessageInput =>
        TransactionStatus != TransactionStatus.Completed;

    public int JourneyActiveIndex => ChatJourneyConfig.GetActiveIndex(TransactionStatus);
}
