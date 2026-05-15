namespace Ketabi.Core.Interfaces;

using Ketabi.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ICategoryRepository Categories { get; }
    IBookListingRepository Listings { get; }
    IRequestRepository Requests { get; }
    IReviewRepository Reviews { get; }
    INotificationRepository Notifications { get; }
    Task<int> SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();

}