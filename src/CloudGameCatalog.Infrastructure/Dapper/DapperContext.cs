using CloudGameCatalog.Infrastructure.Dapper.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace CloudGameCatalog.Infrastructure.Dapper
{
    public class DapperContext(IConfiguration configuration) : IDapperContext
    {
        private readonly string _connectionString = configuration.GetConnectionString("Default");

        public IDbConnection OpenConnection() => new SqlConnection(_connectionString);
    }
}
