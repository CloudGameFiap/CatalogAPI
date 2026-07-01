using CloudGameCatalog.Domain.Entities;
using CloudGameCatalog.Domain.Interfaces;
using CloudGameCatalog.Infrastructure.Dapper.Contracts;
using Dapper;
using System.Data;

namespace CloudGameCatalog.Infrastructure.Dapper.Repositories;

public sealed class UserReadOnlyRepository(IDapperContext context)
    : AbstractRepository<User, int>(context), IUserReadOnlyRepository
{
    public override async Task<User> GetByIdAsync(int id)
    {
        using IDbConnection connection = Context.OpenConnection();
        return await connection.QueryFirstOrDefaultAsync<User>("SELECT Id,Name,Email,Active,IsAdmin,BirthDate,CreatedAt,UpdateAt FROM Users WHERE Id = @id", new { id });
    }
}
