using CloudGameCatalog.Domain.Entities;

namespace CloudGameCatalog.Domain.Interfaces;

public interface IReadOnlyRepository<TEntity, in TId> where TEntity : Entity<TId>
{
    Task<TEntity> GetByIdAsync(TId id);
}
