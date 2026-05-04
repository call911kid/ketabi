using Ketabi.Core.Interfaces;
using Ketabi.Core.Interfaces.Repositories;
using Ketabi.Infrastructure.Persistence;
using Ketabi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ketabi.Infrastructure
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly KetabiDbContext _context;
        private IDbContextTransaction? _transaction;
        private IUserRepository? _users;
        private ICategoryRepository? _categories;
        private IBookListingRepository? _listings;
        private IRequestRepository? _requests;
        private IReviewRepository? _reviews;

        public UnitOfWork(KetabiDbContext context)
        {
            _context = context;
        }
        public IUserRepository Users => _users ??= new UserRepository(_context);
        public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
        public IBookListingRepository Listings => _listings ??= new UserBookRepository(_context);
        public IRequestRepository Requests => _requests ??= new RequestRepository(_context);
        public IReviewRepository Reviews => _reviews ??= new ReviewRepository(_context);
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task BeginTransactionAsync()
        {
            if (_transaction != null) return;
            _transaction = await _context.Database.BeginTransactionAsync();
        }
        public async Task CommitTransactionAsync()
        {
             if (_transaction is null) return;
             await _transaction.CommitAsync();
             await _transaction.DisposeAsync();
             _transaction = null;
        }
        public async Task RollbackTransactionAsync()
        {
             if (_transaction is null) return;
             await _transaction.RollbackAsync();
             await _transaction.DisposeAsync();
             _transaction = null;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
