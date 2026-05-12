using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Domain.Enums;
using Ketabi.Core.Domain.Models;
using Ketabi.Core.Interfaces.Repositories;
using Ketabi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ketabi.Infrastructure.Repositories
{
    internal class BookListingRepository : GenericRepository<BookListing>, IBookListingRepository
    {
        public BookListingRepository(KetabiDbContext context) : base(context)
        {
        }

        private IQueryable<BookListing> QueryWithIncludes()
        {
            return _dbSet.Include(b => b.User).Include(b => b.Category);
        }

        public async Task<PagedResult<BookListing>> GetListingsByLocationAndModeAsync(string governorate, SharingMode mode, int pageNumber, int pageSize)
        {
            var query =  _dbSet
                .Where(ub => ub.IsAvailable && ub.LocationNote.Contains(governorate) && (mode == SharingMode.Both || ub.SharingMode == mode));
            var totalCount = await query
                .CountAsync();
            var items = await query
                .OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<BookListing>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<PagedResult<BookListing>> GetPagedWithIncludesAsync(int pageNumber, int pageSize)
        {
            var query = QueryWithIncludes();
            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<BookListing>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<PagedResult<BookListing>> FindPagedWithIncludesAsync(Expression<Func<BookListing, bool>> predicate, int pageNumber, int pageSize)
        {
            var query = QueryWithIncludes().Where(predicate);
            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<BookListing>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<IEnumerable<BookListing>> FindWithIncludesAsync(Expression<Func<BookListing, bool>> predicate)
        {
            return await QueryWithIncludes().Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<BookListing>> GetAllWithIncludesAsync()
        {
            return await QueryWithIncludes().ToListAsync();
        }

        public async Task<BookListing?> GetByIdWithIncludesAsync(Guid id)
        {
            return await QueryWithIncludes().FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
