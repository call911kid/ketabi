namespace Ketabi.Core.Interfaces;

using Ketabi.Core.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ICategoryRepository Categories { get; }
    IBookListingRepository Listings { get; }
    IRequestRepository Requests { get; }
    IReviewRepository Reviews { get; }

    Task<int> SaveChangesAsync();

    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}