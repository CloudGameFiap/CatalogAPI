using CloudGameCatalog.Domain.Entities;
using CloudGameCatalog.Domain.Interfaces;

namespace CloudGameCatalog.Infrastructure.EntityFramework.Repositories
{
    public sealed class GameWriteOnlyRepository(AppDbContext dbContext) : AbstractRepository<Game, int>(dbContext), IGameWriteOnlyRepository
    {
    }
}
