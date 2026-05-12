namespace Ketabi.Core.Interfaces.Repositories;

using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Domain.Enums;
using Ketabi.Core.Domain.Models;
using System.Linq.Expressions;

public interface IBookListingRepository : IGenericRepository<BookListing>
{

    Task<PagedResult<BookListing>> GetListingsByLocationAndModeAsync(
        string governorate,
        SharingMode mode,
        int pageNumber,
        int pageSize);

    Task<PagedResult<BookListing>> GetPagedWithIncludesAsync(int pageNumber, int pageSize);
    Task<PagedResult<BookListing>> FindPagedWithIncludesAsync(Expression<Func<BookListing, bool>> predicate, int pageNumber, int pageSize);
    Task<IEnumerable<BookListing>> FindWithIncludesAsync(Expression<Func<BookListing, bool>> predicate);
    Task<IEnumerable<BookListing>> GetAllWithIncludesAsync();
    Task<BookListing?> GetByIdWithIncludesAsync(Guid id);
}

