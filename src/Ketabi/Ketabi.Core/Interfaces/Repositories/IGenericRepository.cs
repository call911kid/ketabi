namespace Ketabi.Core.Interfaces.Repositories;

using Ketabi.Core.Domain;
using Ketabi.Core.Domain.Models;
using System.Linq.Expressions;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    Task<IEnumerable<T>> GetAllAsync();

    Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize);

    Task<PagedResult<T>> FindPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize);

    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    Task AddAsync(T entity);

    void Update(T entity);

    void Delete(T entity);
}