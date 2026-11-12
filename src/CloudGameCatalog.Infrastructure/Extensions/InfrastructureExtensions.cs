using CloudGameCatalog.Application.Interfaces;
using CloudGameCatalog.Domain.Interfaces;
using CloudGameCatalog.Infrastructure.Cache;
using CloudGameCatalog.Infrastructure.Dapper;
using CloudGameCatalog.Infrastructure.Dapper.Contracts;
using CloudGameCatalog.Infrastructure.Dapper.Repositories;
using CloudGameCatalog.Infrastructure.EntityFramework;
using CloudGameCatalog.Infrastructure.EntityFramework.Repositories;
using CloudGameCatalog.Infrastructure.MongoDb;
using CloudGameCatalog.Infrastructure.MongoDb.Context;
using CloudGameCatalog.Infrastructure.MongoDb.Repositories;
using CloudGameCatalog.Infrastructure.BackgroundServices;
using CloudGameCatalog.Infrastructure.Search;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
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
        services.AddScoped<IRedisCacheService, RedisCacheService>();

        services.Configure<MongoDbOptions>(configuration.GetSection(MongoDbOptions.SectionName));

        services.AddSingleton<IMongoDbContext, MongoDbContext>();

        services.AddScoped<ICacheService, MongoCacheService>();

        services.Configure<ElasticsearchOptions>(configuration.GetSection(ElasticsearchOptions.SectionName));

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<ElasticsearchOptions>>().Value;

            var settings = new ElasticsearchClientSettings(new Uri(options.Url))
                .DefaultIndex(options.IndexName)
                .RequestTimeout(TimeSpan.FromSeconds(10));

            // Elastic Cloud / cluster com segurança habilitada: API key ou usuário/senha.
            if (!string.IsNullOrWhiteSpace(options.ApiKey))
                settings = settings.Authentication(new ApiKey(options.ApiKey));
            else if (!string.IsNullOrWhiteSpace(options.Username))
                settings = settings.Authentication(new BasicAuthentication(options.Username, options.Password ?? string.Empty));

            return new ElasticsearchClient(settings);
        });

        services.AddSingleton<IGameSearchService, ElasticsearchGameService>();
        services.AddHostedService<SearchIndexInitializer>();

        return services;
    }
}
