namespace Ketabi.Application.DTOs.Chat;

public class SendMessageDto
{
    public Guid ConversationId { get; set; }
    public string Text { get; set; } = string.Empty;
}
