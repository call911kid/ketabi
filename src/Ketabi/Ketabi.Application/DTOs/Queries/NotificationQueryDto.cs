using Ketabi.Application.DTOs.Common;

namespace Ketabi.Application.DTOs.Queries;

/// <summary>Query parameters for the notifications list.</summary>
public class NotificationQueryDto : PagedRequestDto
{
    /// <summary>When true, returns only unread notifications.</summary>
    public bool UnreadOnly { get; init; }
}
