using Ketabi.Core.Interfaces;
using Ketabi.Core.Interfaces.Repositories;
using Ketabi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ketabi.Infrastructure
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly KetabiDbContext _context;
        public IUserRepository Users { get; set; }
        public ICategoryRepository Categories { get; set; }
        public IBookListingRepository Listings { get; set; }
        public IRequestRepository Requests { get; set; }
        public IReviewRepository Reviews { get; set; }

        public UnitOfWork(KetabiDbContext context,
            IUserRepository users,
            ICategoryRepository categories,
            IBookListingRepository listings,
            IRequestRepository requests,
            IReviewRepository reviews
            )
        {
            _context = context;
            Users = users;
            Categories = categories;
            Listings = listings;
            Requests = requests;
            Reviews = reviews;
        }

        public async Task<int> SaveChangesAsync() =>
            await _context.SaveChangesAsync();
        public async Task<IDbContextTransaction> BeginTransactionAsync() =>
            await _context.Database.BeginTransactionAsync();

        public void Dispose() =>
            _context.Dispose();

    }
}
