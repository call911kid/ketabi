namespace Ketabi.Core.Interfaces.Repositories;

using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Domain.Models;

public interface IReviewRepository : IGenericRepository<Review>
{
    // not sure for best practce
    // This method calculates the average rating for a user based on all reviews where they are the target user.
    Task<double> CalculateAverageRatingForUserAsync(Guid userId);
    // Retrieves a paginated list of reviews for a specific user, where they are the target of the reviews.
    Task<PagedResult<Review>> GetReviewsForUserAsync(Guid targetUserId, int pageNumber, int pageSize);
}