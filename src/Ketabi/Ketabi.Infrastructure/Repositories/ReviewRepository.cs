using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Domain.Models;
using Ketabi.Core.Interfaces.Repositories;
using Ketabi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ketabi.Infrastructure.Repositories
{
    internal class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(KetabiDbContext context) : base(context)
        {
        }

        public async Task<double> CalculateAverageRatingForUserAsync(Guid userId)
        {
            var hasReviews = await _dbSet
             .AnyAsync(r => r.TargetUserId == userId);

            if (!hasReviews) return 0;

            return await _dbSet
                .Where(r => r.TargetUserId == userId)
                .AverageAsync(r => r.Rating);

        }
        public async Task<PagedResult<Review>> GetReviewsForUserAsync(Guid targetUserId, int pageNumber, int pageSize)
                => await GetReviewsPagedAsync(r => r.TargetUserId == targetUserId, pageNumber, pageSize);

        public async Task<int> CountReviewsForUserAsync(Guid targetUserId)
        {
            return await _dbSet.CountAsync(r => r.TargetUserId == targetUserId);
        }

        private async Task<PagedResult<Review>> GetReviewsPagedAsync(
            Expression<Func<Review, bool>> predicate, int pageNumber, int pageSize)
        {
            var query = _dbSet
                .Where(predicate)
                .Include(r => r.Reviewer)
                .Include(r => r.TargetUser)
                .Include(r => r.RelatedRequest)
                    .ThenInclude(req => req.Listing);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Review>(items, totalCount, pageNumber, pageSize);
        }
    }
}
