using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Domain.Models;
using Ketabi.Core.Interfaces.Repositories;
using Ketabi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ketabi.Infrastructure.Repositories
{
    internal class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(KetabiDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<User>> GetTopReputationUsersAsync(int count)
        {
            return await _dbSet
                .OrderByDescending(u => u.ReputationScore)
                .Take(count)
                .ToListAsync();
        }

        public async Task<User?> GetUserWithListingsAsync(Guid userId)
        {
            return await _dbSet
                .Include(u => u.Listings)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task UpdateUserReputationAsync(Guid userId, double newScore)
        {
            var user = await _dbSet.FindAsync(userId);
            if (user != null)
            {
                user.ReputationScore = newScore;
                _dbSet.Update(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }
        }
        public async Task<PagedResult<User>> GetPagedAsync(int pageNumber, int pageSize, string? search = null)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.Email.Contains(search)
                                      || u.FirstName.Contains(search)
                                      || u.LastName.Contains(search));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<User>(items, totalCount, pageNumber, pageSize);
        }
    }
}
