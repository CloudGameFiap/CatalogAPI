using CloudGameCatalog.Domain.Entities;
using CloudGameCatalog.Domain.Interfaces;

namespace CloudGameCatalog.Infrastructure.EntityFramework.Repositories
{
    public class UserGameWriteOnlyRepository(AppDbContext dbContext) : AbstractRepository<UserGame, int>(dbContext), IUserGameWriteOnlyRepository
    {
    }
}
