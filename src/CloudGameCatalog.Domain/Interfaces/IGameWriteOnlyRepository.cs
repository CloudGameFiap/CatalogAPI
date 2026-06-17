using CloudGameCatalog.Domain.Entities;

namespace CloudGameCatalog.Domain.Interfaces;

public interface IGameWriteOnlyRepository : IWriteOnlyRepository<Game, int>
{ }