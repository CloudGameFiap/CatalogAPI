using CloudGameCatalog.Domain.Entities;
using CloudGameCatalog.Domain.Interfaces;

namespace CloudGameCatalog.Infrastructure.EntityFramework.Repositories;

public sealed class UserWriteOnlyRepository(AppDbContext dbContext) : AbstractRepository<User, int>(dbContext), IUserWriteOnlyRepository
{
}
