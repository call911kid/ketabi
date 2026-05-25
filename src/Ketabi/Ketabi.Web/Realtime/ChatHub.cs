using Ketabi.Application.DTOs.Chat;
using Ketabi.Application.DTOs.Notifications;
using Ketabi.Application.Interfaces;
using Ketabi.Core.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Ketabi.Web.Realtime;

[Authorize]
public sealed class ChatHub : Hub
{
    private readonly IConversationService _conversationService;
    private readonly INotificationService _notificationService;
    private readonly INotificationDispatcher _dispatcher;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(
        IConversationService conversationService,
        INotificationService notificationService,
        INotificationDispatcher dispatcher,
        ILogger<ChatHub> logger)
    {
        _conversationService = conversationService;
        _notificationService = notificationService;
        _dispatcher = dispatcher;
        _logger = logger;
    }

    // Join
    public async Task JoinConversation(string conversationId)
    {
        if (!TryGetUserId(out var userId)) return;

        if (!Guid.TryParse(conversationId, out var convGuid))
        {
            await Clients.Caller.SendAsync("Error", "Invalid conversation id.");
            return;
        }

        var isParticipant = await _conversationService
            .IsParticipantAsync(convGuid, userId);

        if (!isParticipant) return;

        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        await _conversationService.MarkAsReadAsync(convGuid, userId);
        await Clients.Caller.SendAsync("Joined", conversationId);

        _logger.LogInformation("User {UserId} joined conversation {ConvId}", userId, convGuid);
    }

    // Leave
    public async Task LeaveConversation(string conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
    }

    // Send message
    public async Task SendMessage(string conversationId, string text)
    {
        if (!TryGetUserId(out var userId)) return;

        if (!Guid.TryParse(conversationId, out var convGuid))
        {
            await Clients.Caller.SendAsync("Error", "Invalid conversation id.");
            return;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            await Clients.Caller.SendAsync("Error", "Message cannot be empty.");
            return;
        }

        var dto = new SendMessageDto
        {
            ConversationId = convGuid,
            Text = text.Trim()
        };

        var result = await _conversationService.SendMessageAsync(dto, userId);

        if (!result.Success)
        {
            await Clients.Caller.SendAsync("Error", result.Errors.FirstOrDefault());
            return;
        }

        // Broadcast the neutral DTO (IsOwn not set) — clients will mark ownership locally
        await Clients.Group(conversationId)
                     .SendAsync("ReceiveMessage", result.Data);

        await SendMessageNotificationAsync(convGuid, userId, text);
    }

    // Mark read
    public async Task MarkRead(string conversationId)
    {
        if (!TryGetUserId(out var userId)) return;

        if (!Guid.TryParse(conversationId, out var convGuid)) return;

        await _conversationService.MarkAsReadAsync(convGuid, userId);

        await Clients.OthersInGroup(conversationId)
                     .SendAsync("MessagesRead", conversationId, userId.ToString());
    }

    // Typing 
    public async Task Typing(string conversationId)
    {
        if (!TryGetUserId(out var userId)) return;

        await Clients.OthersInGroup(conversationId)
                     .SendAsync("UserTyping", conversationId, userId.ToString());
    }

    // Confirm handoff
    public async Task ConfirmHandoff(string conversationId)
    {
        if (!TryGetUserId(out var userId)) return;

        if (!Guid.TryParse(conversationId, out var convGuid))
        {
            await Clients.Caller.SendAsync("Error", "Invalid conversation id.");
            return;
        }

        var dto = new ConfirmHandoffDto { ConversationId = convGuid };

        var result = await _conversationService.ConfirmHandoffAsync(dto, userId);

        if (!result.Success)
        {
            await Clients.Caller.SendAsync("Error", result.Errors.FirstOrDefault());
            return;
        }

        await Clients.Group(conversationId)
                     .SendAsync("HandoffConfirmed", conversationId, userId.ToString());

        _logger.LogInformation("User {UserId} confirmed handoff for conversation {ConvId}",
            userId, convGuid);
    }

    // Lifecycle 
    public override async Task OnConnectedAsync()
    {
        _logger.LogDebug("SignalR connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception is not null)
            _logger.LogWarning(exception,
                "SignalR disconnected with error: {ConnectionId}", Context.ConnectionId);
        else
            _logger.LogDebug("SignalR disconnected: {ConnectionId}", Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }

    //  Private: notification on new message
    private async Task SendMessageNotificationAsync(Guid convGuid, Guid senderId, string text)
    {
        try
        {
            var convResult = await _conversationService
                .GetConversationAsync(convGuid, senderId);

            if (!convResult.Success || convResult.Data is null) return;

            var conv = convResult.Data;
            var recipientId = senderId == conv.OwnerId ? conv.RequesterId : conv.OwnerId;

            var senderName = senderId == conv.OwnerId
                ? conv.OwnerName
                : conv.RequesterName;

            var preview = text.Length > 80 ? text[..80] + "…" : text;

            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = recipientId,
                Title = senderName,
                Content = preview,
                NotificationType = NotificationType.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Failed to send message notification for conversation {ConvId}", convGuid);
        }
    }

    // Helpers
    private bool TryGetUserId(out Guid userId)
    {
        userId = Guid.Empty;
        var claim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(claim) || !Guid.TryParse(claim, out userId))
        {
            _logger.LogWarning("Unauthenticated SignalR call from {ConnectionId}",
                Context.ConnectionId);
            return false;
        }

        return true;
    }
}