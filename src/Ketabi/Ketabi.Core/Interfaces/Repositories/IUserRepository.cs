namespace Ketabi.Core.Interfaces.Repositories;

using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Domain.Models;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetUserWithListingsAsync(Guid userId);

    Task<IEnumerable<User>> GetTopReputationUsersAsync(int count);

    Task UpdateUserReputationAsync(Guid userId, double newScore);
    Task<PagedResult<User>> GetPagedAsync(int pageNumber, int pageSize, string? search = null);
}