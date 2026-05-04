namespace Ketabi.Core.Interfaces.Repositories;

using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Domain.Enums;
using Ketabi.Core.Domain.Models;

public interface IBookListingRepository : IGenericRepository<BookListing>
{

    Task<PagedResult<BookListing>> GetListingsByLocationAndModeAsync(
        string governorate,
        SharingMode mode,
        int pageNumber,
        int pageSize);

}