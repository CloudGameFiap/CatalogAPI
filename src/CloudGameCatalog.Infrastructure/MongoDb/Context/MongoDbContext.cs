using CloudGameCatalog.Infrastructure.MongoDb.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CloudGameCatalog.Infrastructure.MongoDb.Context;

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbOptions> options)
    {
        var settings = options.Value;
        var client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoCollection<CacheModel> GetCollection(string collectionName)
    {
        var collection = _database.GetCollection<CacheModel>(collectionName);

        // Garante o índice TTL de forma idempotente (só cria se não existir)
        var indexKeys = Builders<CacheModel>.IndexKeys.Ascending(x => x.ExpiresAt);
        var indexOptions = new CreateIndexOptions { ExpireAfter = TimeSpan.Zero };
        collection.Indexes.CreateOne(new CreateIndexModel<CacheModel>(indexKeys, indexOptions));

        return collection;
    }
}