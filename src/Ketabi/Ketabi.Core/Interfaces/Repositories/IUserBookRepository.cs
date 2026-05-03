namespace Ketabi.Core.Interfaces.Repositories;

using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Domain.Enums;
using Ketabi.Core.Domain.Models;

public interface IUserBookRepository : IGenericRepository<UserBook>
{

    Task<PagedResult<UserBook>> GetListingsByLocationAndModeAsync(
        string governorate,
        SharingMode mode,
        int pageNumber,
        int pageSize);

}