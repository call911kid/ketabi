using Ketabi.Application.DTOs.Notifications;
using Ketabi.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Ketabi.Web.Realtime;

// internal: only the DI container resolves this. Scoped lifetime matches INotificationService.
internal sealed class NotificationDispatcher : INotificationDispatcher
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationDispatcher> _logger;

    public NotificationDispatcher(
        IHubContext<NotificationHub> hubContext,
        ILogger<NotificationDispatcher> logger)
    {
        _hubContext = hubContext;
        _logger     = logger;
    }

    public async Task DispatchAsync(Guid userId, NotificationDto dto)
    {
        try
        {
            // Clients.User resolves via the default IUserIdProvider (ClaimTypes.NameIdentifier).
            // The JWT-in-Cookie middleware guarantees this claim is present on every authenticated connection.
            await _hubContext.Clients
                .User(userId.ToString())
                .SendAsync("notification:received", dto);
        }
        catch (Exception ex)
        {
            // Real-time delivery is best-effort. The notification is already persisted in DB.
            _logger.LogWarning(ex,
                "SignalR dispatch failed for user {UserId}. Notification is persisted in DB.",
                userId);
        }
    }
}
