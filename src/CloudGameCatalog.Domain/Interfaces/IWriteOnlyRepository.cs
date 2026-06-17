using CloudGameCatalog.Domain.Entities;

namespace CloudGameCatalog.Domain.Interfaces;

public interface IWriteOnlyRepository<in TEntity, in TId> where TEntity : Entity<TId>
{
    Task AddAsync(TEntity entity);

    Task UpdateAsync(TEntity entity);

    Task RemoveAsync(TId id);
}