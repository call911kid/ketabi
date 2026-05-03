namespace Ketabi.Core.Interfaces.Repositories;

using Ketabi.Core.Domain.Entities;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetUserWithListingsAsync(Guid userId);

    Task<IEnumerable<User>> GetTopReputationUsersAsync(int count);

    Task UpdateUserReputationAsync(Guid userId, double newScore);
}