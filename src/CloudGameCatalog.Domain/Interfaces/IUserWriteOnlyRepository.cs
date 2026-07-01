using CloudGameCatalog.Domain.Entities;

namespace CloudGameCatalog.Domain.Interfaces;

public interface IUserWriteOnlyRepository : IWriteOnlyRepository<User, int>
{ }