namespace Ketabi.Core.Interfaces.Repositories;

using Ketabi.Core.Domain.Entities;

public interface IReviewRepository : IGenericRepository<Review>
{
    // not sure for best practce
    // This method calculates the average rating for a user based on all reviews where they are the target user.
    Task<double> CalculateAverageRatingForUserAsync(Guid userId);
}