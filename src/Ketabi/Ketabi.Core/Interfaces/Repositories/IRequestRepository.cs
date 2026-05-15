namespace Ketabi.Core.Interfaces.Repositories;

using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Domain.Enums;
using Ketabi.Core.Domain.Models;

public interface IRequestRepository : IGenericRepository<Request>
{
    Task<Request?> GetDetailsAsync(Guid requestId);
    Task<PagedResult<Request>> GetIncomingDetailsAsync(Guid ownerId, RequestStatus? status, int pageNumber, int pageSize);
    Task<PagedResult<Request>> GetOutgoingDetailsAsync(Guid requesterId, RequestStatus? status, int pageNumber, int pageSize);
    Task<bool> HasActiveRequestForListingAsync(Guid listingId);
    Task<bool> IsUserPartyToRequestForListingAsync(Guid userId, Guid listingId);
    Task<IReadOnlyList<Request>> GetPendingRequestsForListingAsync(Guid listingId, Guid excludingRequestId);
    Task<int> CountCompletedTradesForUserAsync(Guid targetUserId);
}
