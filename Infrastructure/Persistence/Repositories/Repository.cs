using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TPI_2026.Application.Abstractions.Interfaces.Repositories;
using TPI_2026.Domain.Common;

namespace TPI_2026.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly DbContext dbContext;
    protected readonly DbSet<T> DbSet;

    public Repository(DbContext newDbContext)
    {
        dbContext = newDbContext
        ?? throw new ArgumentNullException(nameof(dbContext));

        DbSet = dbContext.Set<T>();
    }


    // Propiedad interna que filtra por defecto los registros borrados lógicamente.
    protected IQueryable<T> ActiveSet => DbSet.Where(entity => !entity.IsDeleted);

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Se usa FirstOrDefaultAsync para que el filtro IsDeleted se aplique correctamente.
        return await ActiveSet.FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await ActiveSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await ActiveSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await ActiveSet.AnyAsync(predicate, cancellationToken);
    }

    public void Add(T entity)
    {
        var now = DateTime.UtcNow;
        entity.CreatedAt = now;
        entity.UpdatedAt = now;
        DbSet.Add(entity);
    }
}

