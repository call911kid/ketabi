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

    /// <summary>True when the current user has already confirmed the handoff.</summary>
    public bool CurrentUserConfirmedHandoff { get; set; }

    /// <summary>True when the other party has already confirmed the handoff.</summary>
    public bool OtherUserConfirmedHandoff { get; set; }

    /// <summary>True when the current user has already submitted a review.</summary>
    public bool ReviewAlreadySubmitted { get; set; }

    // ── Computed helpers (used in views) ────────────────────────────
    public bool ShowHandoffBar =>
        TransactionStatus != TransactionStatus.Completed && !CurrentUserConfirmedHandoff;

    public bool ShowReviewForm =>
        TransactionStatus == TransactionStatus.Completed && !ReviewAlreadySubmitted;

    public bool ShowMessageInput =>
        TransactionStatus != TransactionStatus.Completed;

    public int JourneyActiveIndex => ChatJourneyConfig.GetActiveIndex(TransactionStatus);
}
