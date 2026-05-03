using System.Linq.Expressions;
using Ketabi.Core.Domain;
using Ketabi.Core.Domain.Models;
using Ketabi.Core.Interfaces.Repositories;
using Ketabi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ketabi.Infrastructure.Repositories;

internal class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly KetabiDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(KetabiDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var totalCount = await _dbSet.CountAsync();
        var items = await _dbSet.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<T>> FindPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize)
    {
        var query = _dbSet.Where(predicate);
        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}
