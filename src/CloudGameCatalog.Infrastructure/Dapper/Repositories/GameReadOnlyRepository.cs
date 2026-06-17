using CloudGameCatalog.Domain.Commom;
using CloudGameCatalog.Domain.Entities;
using CloudGameCatalog.Domain.Interfaces;
using CloudGameCatalog.Domain.Parameters;
using CloudGameCatalog.Infrastructure.Dapper.Contracts;
using Dapper;
using System.Data;

namespace CloudGameCatalog.Infrastructure.Dapper.Repositories;

public sealed class GameReadOnlyRepository(IDapperContext context)
    : AbstractRepository<Game, int>(context), IGameReadOnlyRepository
{    

    public async Task<Pagination<Game>> FindAsync(FindGamesParameter parameters)
    {
        var sqlBuilder = new SqlBuilder();        

        if (parameters.Active.HasValue)
            sqlBuilder.Where("Active=@active", new { active = parameters.Active });

        if (!string.IsNullOrWhiteSpace(parameters.Name))
            sqlBuilder.Where("Name LIKE @name", new { name = $"%{parameters.Name}%" });

        var countQuery = sqlBuilder.AddTemplate("Select count(*) from Game /**where**/");
        var queryGames = sqlBuilder.AddTemplate(@"select Id, Name, Active from Game
                                                /**where**/ ORDER BY Id OFFSET @skip ROWS FETCH NEXT @size ROWS ONLY",
                                                new { skip = parameters.Skip, size = parameters.PageSize });

        using IDbConnection connection = Context.OpenConnection();

        int count = await connection.ExecuteScalarAsync<int>(countQuery.RawSql, countQuery.Parameters);
        var games = (await connection.QueryAsync<Game>(queryGames.RawSql, queryGames.Parameters)).ToList();

        return new Pagination<Game>(games, count);
    }    
}
