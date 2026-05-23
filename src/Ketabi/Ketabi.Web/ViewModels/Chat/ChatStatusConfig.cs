namespace Ketabi.Web.ViewModels.Chat;

/// <summary>
/// Maps TransactionStatus to display properties used in the StatusBanner partial
/// and the ConversationItem status pill.
/// </summary>
public static class ChatStatusConfig
{
    public record StatusDisplay(
        string Label,
        string Sublabel,
        string CssClass,       // BEM modifier: "chat-status--active", etc.
        string DotCssClass     // BEM modifier for the dot color
    );

    public static readonly IReadOnlyDictionary<TransactionStatus, StatusDisplay> Map =
        new Dictionary<TransactionStatus, StatusDisplay>
        {
            [TransactionStatus.Active] = new(
                "Chat Active",
                "Message each other to arrange a meetup",
                "chat-status--active",
                "chat-status__dot--indigo"),

            [TransactionStatus.MeetupPending] = new(
                "Meetup Pending",
                "Agree on a time &amp; place, then confirm the handoff below",
                "chat-status--meetup",
                "chat-status__dot--warning"),

            [TransactionStatus.HandoffConfirmedRequester] = new(
                "Waiting for Owner",
                "You&#39;ve confirmed — waiting for the other party to confirm",
                "chat-status--waiting",
                "chat-status__dot--info"),

            [TransactionStatus.HandoffConfirmedOwner] = new(
                "Waiting for You",
                "The owner confirmed. Please confirm the handoff below",
                "chat-status--waiting",
                "chat-status__dot--info"),

            [TransactionStatus.Completed] = new(
                "Completed 🎉",
                "Both parties confirmed the handoff. Leave a review below!",
                "chat-status--completed",
                "chat-status__dot--success"),
        };
}
