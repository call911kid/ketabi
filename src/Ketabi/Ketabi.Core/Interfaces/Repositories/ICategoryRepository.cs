namespace Ketabi.Core.Interfaces.Repositories;

using Ketabi.Core.Domain.Entities;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IEnumerable<Category>> GetCategoriesWithBooksAsync();
}