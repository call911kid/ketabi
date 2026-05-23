namespace Ketabi.Web.ViewModels.Chat;

/// <summary>
/// Root ViewModel for the Chat page. Drives both the sidebar list
/// and the active conversation panel.
/// </summary>
public class ChatIndexViewModel
{
    /// <summary>All conversations for the current user (sidebar list).</summary>
    public IList<ConversationSummaryViewModel> Conversations { get; set; } = [];

    /// <summary>The currently selected/open conversation. Null = no selection.</summary>
    public ConversationDetailViewModel? ActiveConversation { get; set; }

    /// <summary>Total active (non-completed) conversation count — shown in sidebar badge.</summary>
    public int ActiveConversationCount => Conversations.Count(c => c.TransactionStatus != TransactionStatus.Completed);
}
