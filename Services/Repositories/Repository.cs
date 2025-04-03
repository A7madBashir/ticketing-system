using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Data;
using TicketingSystem.Models.Common.BaseEntity;

namespace TicketingSystem.Services.Repositories;

public interface IRepository<TEntity, T>
    where T : IEquatable<T>
    where TEntity : class, IEntity<T>
{
    Task AddAsync(TEntity entity);
    Task DeleteAsync(T id);
    Task UpdateAsync(TEntity entity);
    Task<bool> ExistAsync(T id);
    Task<bool> ExistAsync(System.Linq.Expressions.Expression<Func<TEntity, bool>> expression);

    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> FirstOrDefaultAsync(
        System.Linq.Expressions.Expression<Func<TEntity, bool>> expression
    );
    Task<TEntity?> GetByIdAsync(T id, IEnumerable<string>? navigationProperties = null);
    IQueryable<TEntity> Query(IEnumerable<string>? navigationProperties = null);
    DbSet<TEntity> QueryWithTracking();

    Task SaveChangesAsync();
}

public class Repository<TEntity, T>(ApplicationDbContext context) : IRepository<TEntity, T>
    where T : IEquatable<T>
    where TEntity : class, IEntity<T>
{
    private readonly ApplicationDbContext _context = context;
    protected readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T id)
    {
        var item = await _dbSet.FindAsync(id);
        if (item is null)
        {
            return;
        }

        _dbSet.Remove(item);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistAsync(T id)
    {
        bool exist = await _dbSet.AnyAsync(t => id.Equals(t.Id));
        return exist;
    }

    public async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> expression)
    {
        bool exist = await _dbSet.AnyAsync(expression);
        return exist;
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression)
    {
        var item = await _dbSet.FirstOrDefaultAsync(expression);
        return item;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(T id, IEnumerable<string>? navigationProperties = null)
    {
        IQueryable<TEntity> queryable = _dbSet.Where(e => id.Equals(e.Id)).AsQueryable();
        if (navigationProperties is not null)
        {
            foreach (var property in navigationProperties)
            {
                queryable = queryable.Include(property);
            }
        }

        return await queryable.FirstOrDefaultAsync();
    }

    public IQueryable<TEntity> Query(IEnumerable<string>? navigationProperties = null)
    {
        if (navigationProperties is null)
        {
            return _dbSet.AsNoTracking();
        }

        var query = _dbSet.AsQueryable();
        foreach (var property in navigationProperties)
        {
            query = query.Include(property);
        }

        return query.AsNoTracking();
    }

    public DbSet<TEntity> QueryWithTracking()
    {
        return _dbSet;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
    }
}
