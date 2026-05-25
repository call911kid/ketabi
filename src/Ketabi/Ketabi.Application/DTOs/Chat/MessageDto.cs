namespace Ketabi.Application.DTOs.Chat;

public class MessageDto
{
    public Guid MessageId { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string SenderAvatar { get; set; } = string.Empty;
    public bool IsOwn { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}