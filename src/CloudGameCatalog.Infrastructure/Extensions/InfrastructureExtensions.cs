using CloudGameCatalog.Domain.Interfaces;
using CloudGameCatalog.Infrastructure.Dapper;
using CloudGameCatalog.Infrastructure.Dapper.Contracts;
using CloudGameCatalog.Infrastructure.Dapper.Repositories;
using CloudGameCatalog.Infrastructure.EntityFramework;
using CloudGameCatalog.Infrastructure.EntityFramework.Repositories;
using CloudGameCatalog.Infrastructure.MongoDb;
using CloudGameCatalog.Infrastructure.MongoDb.Context;
using CloudGameCatalog.Infrastructure.MongoDb.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace CloudGameCatalog.Infrastructure.Extensions;

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
        services.AddScoped<IUserGameWriteOnlyRepository, UserGameWriteOnlyRepository>();
        services.AddScoped<IUserGameReadOnlyRepository, UserGameReadOnlyRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>(sp => new UnitOfWork(sp.GetRequiredService<AppDbContext>()));

        // 1. Mapeia a seção do appsettings/env
        services.Configure<MongoDbOptions>(configuration.GetSection("MongoDbCache"));

        // 2. Contexto como Singleton (MongoClient gerencia o pool de conexões internamente)
        services.AddSingleton<IMongoDbContext, MongoDbContext>();

        // 3. Serviço de Cache
        services.AddScoped<ICacheService, MongoCacheService>();

        return services;
    }
}
