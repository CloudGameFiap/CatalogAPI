using System.Data;

namespace CloudGameCatalog.Infrastructure.Dapper.Contracts
{
    public interface IDapperContext
    {
        IDbConnection OpenConnection();
    }
}
