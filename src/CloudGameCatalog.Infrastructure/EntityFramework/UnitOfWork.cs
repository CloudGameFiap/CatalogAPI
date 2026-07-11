using CloudGameCatalog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace CloudGameCatalog.Infrastructure.EntityFramework
{
    public sealed class UnitOfWork(DbContext dbContext) : IUnitOfWork
    {
        private readonly DbContext _dbContext = dbContext;

        public async Task<IDbTransaction> BeginTransationAsync(IsolationLevel isolationLevel)
        {
            var efIsolationLevel = (System.Data.IsolationLevel)isolationLevel;

            var transaction = await _dbContext.Database.BeginTransactionAsync(efIsolationLevel);

            return transaction.GetDbTransaction();
        }

        public async Task<IDbTransaction> BeginTransationAsync()
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync();

            return transaction.GetDbTransaction();
        }

        public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
    }
}
