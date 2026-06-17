using System.Data;

namespace CloudGameCatalog.Domain.Interfaces;

public interface IUnitOfWork
{
    Task<IDbTransaction> BeginTransationAsync(IsolationLevel isolationLevel);

    Task<IDbTransaction> BeginTransationAsync();

    Task SaveChangesAsync();
}