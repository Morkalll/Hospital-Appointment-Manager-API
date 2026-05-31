using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TPI_2026.Application.Abstractions.Interfaces.Repositories;

namespace TPI_2026.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly DbContext dbContext;
    protected readonly DbSet<T> DbSet;

    public Repository(DbContext newDbContext)
    {
        dbContext = newDbContext
        ?? throw new ArgumentNullException(nameof(dbContext));

        DbSet = dbContext.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(predicate, cancellationToken);
    }

    public void Add(T entity)
    {
        DbSet.Add(entity);
    }

    public void Remove(T entity)
    {
        DbSet.Remove(entity);
    }
}

