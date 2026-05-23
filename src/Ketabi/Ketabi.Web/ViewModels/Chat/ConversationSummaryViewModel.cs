namespace Ketabi.Web.ViewModels.Chat;

public class ConversationSummaryViewModel
{
    public string ConversationId { get; set; } = string.Empty;
    public BookSummaryViewModel Book { get; set; } = new();
    public OtherUserViewModel OtherUser { get; set; } = new();
    public TransactionStatus TransactionStatus { get; set; }

    /// <summary>Last message text (truncated), for sidebar preview.</summary>
    public string LastMessagePreview { get; set; } = string.Empty;

    /// <summary>True when the last message was sent by the current user.</summary>
    public bool LastMessageIsMine { get; set; }

    /// <summary>Human-readable relative timestamp: "5h ago", "Yesterday".</summary>
    public string LastMessageTimeAgo { get; set; } = string.Empty;

    /// <summary>True when this conversation is the active/selected one.</summary>
    public bool IsSelected { get; set; }
}
