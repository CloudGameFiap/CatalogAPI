using CloudGameCatalog.Domain.Entities;
using CloudGameCatalog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CloudGameCatalog.Infrastructure.EntityFramework.Repositories;

public abstract class AbstractRepository<TEntity, TId> : IWriteOnlyRepository<TEntity, TId> where TEntity : Entity<TId>
{
    protected readonly DbSet<TEntity> DbSet;

    protected readonly DbContext DbContext;

    protected AbstractRepository(DbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = DbContext.Set<TEntity>();
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
    }

    public virtual async Task RemoveAsync(TId id)
    {
        TEntity entity = await DbSet.FindAsync(id);
        DbSet.Remove(entity);
    }

    public virtual async Task UpdateAsync(TEntity entityToUpdate)
    {
        (await DbSet.AddAsync(entityToUpdate)).State = EntityState.Modified;
    }
}

