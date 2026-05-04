using Ketabi.Core.Domain.Entities;
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
    }
}
