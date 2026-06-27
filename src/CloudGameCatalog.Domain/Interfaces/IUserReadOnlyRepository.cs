using CloudGameCatalog.Domain.Entities;

namespace CloudGameCatalog.Domain.Interfaces;

public interface IUserReadOnlyRepository : IReadOnlyRepository<User, int>
{
}
