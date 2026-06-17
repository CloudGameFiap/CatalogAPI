using CloudGameCatalog.Domain.Commom;
using CloudGameCatalog.Domain.Entities;
using CloudGameCatalog.Domain.Parameters;

namespace CloudGameCatalog.Domain.Interfaces
{
    public interface IGameReadOnlyRepository : IReadOnlyRepository<Game, int>
    {       
        Task<Pagination<Game>> FindAsync(FindGamesParameter parameters);
    }
}
