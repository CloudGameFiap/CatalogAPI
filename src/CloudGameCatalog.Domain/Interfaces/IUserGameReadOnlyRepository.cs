using CloudGameCatalog.Domain.Entities;

namespace CloudGameCatalog.Domain.Interfaces
{
    public interface IUserGameReadOnlyRepository : IReadOnlyRepository<UserGame, int>
    {
        Task<IEnumerable<UserGame>> GetByUserIdAsync(int userId);

        Task<UserGame?> GetByUserIdAndGameIdAsync(int userId, int gameId);
    }
}
