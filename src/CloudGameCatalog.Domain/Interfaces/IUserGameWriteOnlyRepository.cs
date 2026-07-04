using CloudGameCatalog.Domain.Entities;

namespace CloudGameCatalog.Domain.Interfaces
{
    public interface IUserGameWriteOnlyRepository : IWriteOnlyRepository<UserGame, int>
    {
    }
}
