using CloudGameCatalog.Domain.Entities;
using CloudGameCatalog.Domain.Interfaces;
using CloudGameCatalog.Infrastructure.Dapper.Contracts;
using System.Data;
using Dapper.Contrib.Extensions;

namespace CloudGameCatalog.Infrastructure.Dapper.Repositories;

public abstract class AbstractRepository<TEntity, TId> : IReadOnlyRepository<TEntity, TId> where TEntity : Entity<TId>
{
    protected readonly IDapperContext Context;

    protected AbstractRepository(IDapperContext context)
    {
        Context = context;
    }

    public virtual async Task<TEntity> GetByIdAsync(TId id)
    {
        using IDbConnection connection = Context.OpenConnection();
        return await connection.GetAsync<TEntity>(id);
    }
}
