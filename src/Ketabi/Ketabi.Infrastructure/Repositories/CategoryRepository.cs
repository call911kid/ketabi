using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Interfaces.Repositories;
using Ketabi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ketabi.Infrastructure.Repositories;

internal class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(KetabiDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetCategoriesWithBooksAsync()
    {
        return await _dbSet.Include(c => c.BookListings).ToListAsync();
    }
}
