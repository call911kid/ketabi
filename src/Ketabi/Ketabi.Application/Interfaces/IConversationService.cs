using Ketabi.Application.DTOs.Chat;
using Ketabi.Application.DTOs.Common;

namespace Ketabi.Application.Interfaces;

public interface IConversationService
{
    Task<ServiceResultDto<ConversationDto>> GetConversationAsync(Guid conversationId, Guid callerId);
    Task<ServiceResultDto<IEnumerable<ConversationDto>>> GetMyConversationsAsync(Guid callerId);
    Task<ServiceResultDto<ConversationDto>> OpenConversationAsync(Guid requestId, Guid callerId);
    Task<ServiceResultDto<MessageDto>> SendMessageAsync(SendMessageDto dto, Guid callerId);
    Task<ServiceResultDto<bool>> MarkAsReadAsync(Guid conversationId, Guid callerId);
    Task<ServiceResultDto<bool>> ConfirmHandoffAsync(ConfirmHandoffDto dto, Guid callerId);
    Task<bool> IsParticipantAsync(Guid conversationId, Guid userId);
}
