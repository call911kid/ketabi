using AutoMapper;
using Ketabi.Application.DTOs.Chat;
using Ketabi.Application.DTOs.Common;
using Ketabi.Application.Interfaces;
using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Domain.Enums;
using Ketabi.Core.Interfaces;

namespace Ketabi.Application.Services;

public class ConversationService : IConversationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ConversationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // Get single conversation
    public async Task<ServiceResultDto<ConversationDto>> GetConversationAsync(
        Guid conversationId, Guid callerId)
    {
        var isParticipant = await _unitOfWork.Conversations
            .IsParticipantAsync(conversationId, callerId);

        if (!isParticipant)
            return ServiceResultDto<ConversationDto>.Fail("You are not a participant in this conversation.");

        var conversation = await _unitOfWork.Conversations
            .GetWithDetailsAsync(conversationId);

        if (conversation is null)
            return ServiceResultDto<ConversationDto>.Fail("Conversation not found.");

        var dto = MapConversation(conversation, callerId);

        return ServiceResultDto<ConversationDto>.Ok(dto);
    }

    // Get all conversations for current user
    public async Task<ServiceResultDto<IEnumerable<ConversationDto>>> GetMyConversationsAsync(
        Guid callerId)
    {
        var conversations = await _unitOfWork.Conversations
            .GetConversationsForUserAsync(callerId);

        var dtos = conversations.Select(c => MapConversation(c, callerId));

        return ServiceResultDto<IEnumerable<ConversationDto>>.Ok(dtos);
    }

    // Open conversation
    public async Task<ServiceResultDto<ConversationDto>> OpenConversationAsync(
        Guid requestId, Guid callerId)
    {
        var existing = await _unitOfWork.Conversations
            .GetByRequestIdAsync(requestId);

        if (existing is not null)
        {
            var isParticipant = await _unitOfWork.Conversations
                .IsParticipantAsync(existing.Id, callerId);

            if (!isParticipant)
                return ServiceResultDto<ConversationDto>.Fail("You are not a participant in this conversation.");

            var existingDto = MapConversation(existing, callerId);
            return ServiceResultDto<ConversationDto>.Ok(existingDto);
        }

        var request = await _unitOfWork.Requests.GetByIdAsync(requestId);

        if (request is null)
            return ServiceResultDto<ConversationDto>.Fail("Request not found.");

        if (request.SenderId != callerId && request.ReceiverId != callerId)
            return ServiceResultDto<ConversationDto>.Fail("You are not a participant in this request.");

        if (request.Status != RequestStatus.Approved)
            return ServiceResultDto<ConversationDto>.Fail("Conversation can only be opened after the request is accepted.");

        var conversation = new Conversation
        {
            OwnerId = request.ReceiverId,
            RequesterId = request.SenderId,
            RequestId = requestId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Conversations.AddAsync(conversation);
        await _unitOfWork.SaveChangesAsync();

        var created = await _unitOfWork.Conversations
            .GetWithDetailsAsync(conversation.Id);

        var dto = MapConversation(created!, callerId);
        return ServiceResultDto<ConversationDto>.Ok(dto);
    }

    // Send message 
    public async Task<ServiceResultDto<MessageDto>> SendMessageAsync(
        SendMessageDto dto, Guid callerId)
    {
        if (string.IsNullOrWhiteSpace(dto.Text))
            return ServiceResultDto<MessageDto>.Fail("Message text cannot be empty.");

        var isParticipant = await _unitOfWork.Conversations
            .IsParticipantAsync(dto.ConversationId, callerId);

        if (!isParticipant)
            return ServiceResultDto<MessageDto>.Fail("You are not a participant in this conversation.");

        var message = new Message
        {
            ConversationId = dto.ConversationId,
            SenderId = callerId,
            Text = dto.Text.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Messages.AddAsync(message);

        var conversation = await _unitOfWork.Conversations
            .GetByIdAsync(dto.ConversationId);

        if (conversation is not null)
        {
            conversation.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Conversations.Update(conversation);
        }

        await _unitOfWork.SaveChangesAsync();

        var messages = await _unitOfWork.Messages
            .GetMessagesForConversationAsync(dto.ConversationId);

        var saved = messages.LastOrDefault(m => m.SenderId == callerId);

        if (saved is null)
            return ServiceResultDto<MessageDto>.Fail("Message could not be retrieved after saving.");

        var messageDto = _mapper.Map<MessageDto>(saved);

        // Do not set IsOwn here. The client will decide ownership by comparing
        // the message sender id with the current user id. Avoid server-side
        // ownership flag to prevent broadcasting a message that appears as
        // 'own' for all group members.
        return ServiceResultDto<MessageDto>.Ok(messageDto);
    }

    // Mark as read 
    public async Task<ServiceResultDto<bool>> MarkAsReadAsync(
        Guid conversationId, Guid callerId)
    {
        var isParticipant = await _unitOfWork.Conversations
            .IsParticipantAsync(conversationId, callerId);

        if (!isParticipant)
            return ServiceResultDto<bool>.Fail("You are not a participant in this conversation.");

        await _unitOfWork.Messages.MarkAllAsReadAsync(conversationId, callerId);

        return ServiceResultDto<bool>.Ok(true);
    }

    // Confirm handoff
    public async Task<ServiceResultDto<bool>> ConfirmHandoffAsync(
        ConfirmHandoffDto dto, Guid callerId)
    {
        var conversation = await _unitOfWork.Conversations
            .GetByIdAsync(dto.ConversationId);

        if (conversation is null)
            return ServiceResultDto<bool>.Fail("Conversation not found.");

        if (conversation.OwnerId != callerId && conversation.RequesterId != callerId)
            return ServiceResultDto<bool>.Fail("You are not a participant in this conversation.");

        if (conversation.OwnerId == callerId)
            conversation.OwnerConfirmedHandoff = true;
        else
            conversation.RequesterConfirmedHandoff = true;

        if (conversation.OwnerConfirmedHandoff && conversation.RequesterConfirmedHandoff)
        {
            var request = await _unitOfWork.Requests.GetByIdAsync(conversation.RequestId);
            if (request is not null)
            {
                request.Status = RequestStatus.Completed;
                request.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Requests.Update(request);
            }
        }

        conversation.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Conversations.Update(conversation);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResultDto<bool>.Ok(true);
    }

    // IsParticipant (used by ChatHub directly)
    public Task<bool> IsParticipantAsync(Guid conversationId, Guid userId) =>
        _unitOfWork.Conversations.IsParticipantAsync(conversationId, userId);

    // Private mapping helper
    private ConversationDto MapConversation(Conversation conversation, Guid callerId)
    {
        var dto = _mapper.Map<ConversationDto>(conversation);

        dto.UnreadCount = conversation.Messages
            .Count(m => !m.IsRead && m.SenderId != callerId);

        // Populate book image url from related request/listing if available
        dto.BookImageUrl = conversation.Request?.Listing?.ImageUrl ?? string.Empty;

        foreach (var msg in dto.Messages)
            msg.IsOwn = msg.SenderId == callerId;

        if (dto.LastMessage is not null)
            dto.LastMessage.IsOwn = dto.LastMessage.SenderId == callerId;

        return dto;
    }
}