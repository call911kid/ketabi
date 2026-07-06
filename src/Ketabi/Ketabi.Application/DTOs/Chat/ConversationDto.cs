namespace Ketabi.Application.DTOs.Chat;

public class ConversationDto
{
    public Guid ConversationId { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string OwnerAvatar { get; set; } = string.Empty;
    public Guid RequesterId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public string RequesterAvatar { get; set; } = string.Empty;
    public Guid RequestId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string BookImageUrl { get; set; } = string.Empty;
    public bool RequesterConfirmedHandoff { get; set; }
    public bool OwnerConfirmedHandoff { get; set; }
    public int UnreadCount { get; set; }
    public MessageDto? LastMessage { get; set; }
    public IEnumerable<MessageDto> Messages { get; set; } = new List<MessageDto>();
    public DateTime CreatedAt { get; set; }
    public string RequestType { get; set; } = string.Empty;
    public int? BorrowDurationDays { get; set; }
}