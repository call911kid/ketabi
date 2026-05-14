namespace Ketabi.Core.Interfaces.Repositories;

using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Domain.Models;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IEnumerable<Category>> GetCategoriesWithBooksAsync();
    Task<IReadOnlyList<CategoryListingCount>> GetTopCategoryListingCountsAsync(int count);
}
