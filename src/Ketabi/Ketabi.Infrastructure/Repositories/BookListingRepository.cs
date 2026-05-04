using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Domain.Enums;
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
    internal class BookListingRepository : GenericRepository<BookListing>, IBookListingRepository
    {
        public BookListingRepository(KetabiDbContext context) : base(context)
        {
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
    }
}
