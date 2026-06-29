using CloudGameCatalog.Domain.Interfaces;
using CloudGameCatalog.Infrastructure.Dapper;
using CloudGameCatalog.Infrastructure.Dapper.Contracts;
using CloudGameCatalog.Infrastructure.Dapper.Repositories;
using CloudGameCatalog.Infrastructure.EntityFramework;
using CloudGameCatalog.Infrastructure.EntityFramework.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace CloudGameCatalog.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer
                (
                    configuration.GetConnectionString("Default")
                )
            );

            services.AddScoped<IDbConnection>(sp => new SqlConnection(configuration.GetConnectionString("Default")));
            services.AddScoped<IDapperContext>(sp => new DapperContext(configuration));
            services.AddScoped<IGameWriteOnlyRepository, GameWriteOnlyRepository>();
            services.AddScoped<IGameReadOnlyRepository, GameReadOnlyRepository>();
            services.AddScoped<IUserReadOnlyRepository, UserReadOnlyRepository>();
            services.AddScoped<IUserWriteOnlyRepository, UserWriteOnlyRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>(sp => new UnitOfWork(sp.GetRequiredService<AppDbContext>()));

            return services;
        }
    }
}
