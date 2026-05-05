using Ketabi.Application.DTOs.Common;
using Ketabi.Application.DTOs.Queries;
using Ketabi.Application.DTOs.Requests;

namespace Ketabi.Application.Interfaces;

public interface IRequestService
{
    Task<RequestDto> CreateBorrowRequestAsync(Guid requesterId, CreateBorrowRequestDto dto);
    Task<RequestDto> CreateExchangeRequestAsync(Guid requesterId, CreateExchangeRequestDto dto);
    Task<PagedResponseDto<RequestDto>> GetIncomingRequestsAsync(Guid ownerId, RequestQueryDto query);
    Task<PagedResponseDto<RequestDto>> GetOutgoingRequestsAsync(Guid requesterId, RequestQueryDto query);
    Task<RequestDto> GetRequestByIdAsync(Guid requestId, Guid userId);
    Task<RequestDto> UpdateRequestStatusAsync(Guid ownerId, Guid requestId, UpdateRequestStatusDto dto);
    Task<RequestDto> CancelRequestAsync(Guid requesterId, Guid requestId);
    Task<RequestDto> CompleteRequestAsync(Guid userId, Guid requestId);
}
