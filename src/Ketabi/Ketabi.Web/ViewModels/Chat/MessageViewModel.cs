namespace Ketabi.Web.ViewModels.Chat;

public class MessageViewModel
{
    public string MessageId { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderAvatarUrl { get; set; } = string.Empty;

    /// <summary>True when the message was sent by the currently authenticated user.</summary>
    public bool IsMine { get; set; }

    public string Text { get; set; } = string.Empty;

    /// <summary>Formatted display time: "01:35 PM".</summary>
    public string FormattedTime { get; set; } = string.Empty;

    /// <summary>Formatted date label for date-divider: "Today", "Yesterday", "May 12".</summary>
    public string DateLabel { get; set; } = string.Empty;

    /// <summary>True when this message starts a new calendar day (drives the date divider).</summary>
    public bool ShowDateDivider { get; set; }

    /// <summary>
    /// True when this message is the last in a consecutive run from the same sender
    /// (or is the final message overall). Drives timestamp visibility — avoids showing
    /// a timestamp after every single bubble when multiple messages are sent in a row.
    /// </summary>
    public bool ShowTimestamp { get; set; }
}
