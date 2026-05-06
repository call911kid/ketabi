using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Domain.Enums;
using Ketabi.Core.Domain.Models;
using Ketabi.Core.Interfaces.Repositories;
using Ketabi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ketabi.Infrastructure.Repositories;

internal class RequestRepository : GenericRepository<Request>, IRequestRepository
{
    public RequestRepository(KetabiDbContext context) : base(context)
    {
    }

    public async Task<Request?> GetDetailsAsync(Guid requestId)
    {
        return await WithDetails()
            .FirstOrDefaultAsync(r => r.Id == requestId);
    }

    public async Task<PagedResult<Request>> GetIncomingDetailsAsync(Guid ownerId, RequestStatus? status, int pageNumber, int pageSize)
    {
        var query = WithDetails()
            .Where(r => r.ReceiverId == ownerId);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        return await ToPagedResultAsync(query, pageNumber, pageSize);
    }

    public async Task<PagedResult<Request>> GetOutgoingDetailsAsync(Guid requesterId, RequestStatus? status, int pageNumber, int pageSize)
    {
        var query = WithDetails()
            .Where(r => r.SenderId == requesterId);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        return await ToPagedResultAsync(query, pageNumber, pageSize);
    }

    public async Task<bool> HasActiveRequestForListingAsync(Guid listingId)
    {
        return await _dbSet.AnyAsync(r =>
            (r.ListingId == listingId || r.OfferedListingId == listingId) &&
            (r.Status == RequestStatus.Pending || r.Status == RequestStatus.Approved));
    }

    public async Task<IReadOnlyList<Request>> GetPendingRequestsForListingAsync(Guid listingId, Guid excludingRequestId)
    {
        return await _dbSet
            .Where(r => (r.ListingId == listingId || r.OfferedListingId == listingId) &&
                        r.Id != excludingRequestId &&
                        r.Status == RequestStatus.Pending)
            .ToListAsync();
    }

    private IQueryable<Request> WithDetails()
    {
        return _dbSet
            .Include(r => r.Listing)
                .ThenInclude(l => l!.Category)
            .Include(r => r.Sender)
            .Include(r => r.Receiver)
            .Include(r => r.OfferedListing)
                .ThenInclude(l => l!.Category);
    }

    private static async Task<PagedResult<Request>> ToPagedResultAsync(IQueryable<Request> query, int pageNumber, int pageSize)
    {
        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(r => r.RequestDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Request>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<int> CountCompletedTradesForUserAsync(Guid targetUserId)
    {
        return await _dbSet.CountAsync(r =>
            (r.SenderId == targetUserId || r.ReceiverId == targetUserId) &&
            r.Status == RequestStatus.Completed);
    }
}
