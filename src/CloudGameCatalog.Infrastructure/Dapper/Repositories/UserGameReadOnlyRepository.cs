using CloudGameCatalog.Domain.Entities;
using CloudGameCatalog.Domain.Interfaces;
using CloudGameCatalog.Infrastructure.Dapper.Contracts;
using System.Data;
using Dapper;

namespace CloudGameCatalog.Infrastructure.Dapper.Repositories
{
    public sealed class UserGameReadOnlyRepository(IDapperContext context) : AbstractRepository<UserGame, int>(context), IUserGameReadOnlyRepository
    {
        public async Task<UserGame?> GetByUserIdAndGameIdAsync(int userId, int gameId)
        {
            const string query = "SELECT * FROM UserGames WHERE UserId = @UserId AND GameId = @GameId";

            using IDbConnection connection = Context.OpenConnection();

            return await connection.QueryFirstOrDefaultAsync<UserGame>(query, new { UserId = userId, GameId = gameId });
        }

        public async Task<IEnumerable<UserGame>> GetByUserIdAsync(int userId)
        {
            const string query = "SELECT * FROM UserGames WHERE UserId = @UserId";

            using IDbConnection connection = Context.OpenConnection();

            return await connection.QueryAsync<UserGame>(query, new { UserId = userId });
        }
    }
}
