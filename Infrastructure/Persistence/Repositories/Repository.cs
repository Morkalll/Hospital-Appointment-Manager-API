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


    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbSet.FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await DbSet.FirstOrDefaultAsync(predicate, cancellationToken);

    public virtual async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await DbSet.ToListAsync(cancellationToken);

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(predicate, cancellationToken);


    public void Add(T entity)
    {
        DbSet.Add(entity);
    }

    // Borrado lógico. El cambio se persiste en el próximo SaveChangesAsync del UnitOfWork.
    public void Remove(T entity)
    {
        entity.SoftDelete(DateTime.UtcNow);
    }
}

