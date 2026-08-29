using CloudGameCatalog.Domain.Interfaces;
using CloudGameCatalog.Infrastructure.MongoDb.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Text.Json;

namespace CloudGameCatalog.Infrastructure.MongoDb.Repositories;

public class MongoGameCacheService : IGameCacheService
{
    private readonly IMongoCollection<GameCacheModel> _cacheCollection;

    public MongoGameCacheService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDbCache:ConnectionString");
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("CloudGameCache");

        _cacheCollection = database.GetCollection<GameCacheModel>("CloudGame");

        // Cria o índice TTL no Mongo para expurgar documentos automaticamente
        var indexKeys = Builders<GameCacheModel>.IndexKeys.Ascending(x => x.ExpiresAt);
        var indexOptions = new CreateIndexOptions { ExpireAfter = TimeSpan.Zero };
        _cacheCollection.Indexes.CreateOne(new CreateIndexModel<GameCacheModel>(indexKeys, indexOptions));
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var filter = Builders<GameCacheModel>.Filter.Eq(x => x.Id, key) &
                     Builders<GameCacheModel>.Filter.Gt(x => x.ExpiresAt, DateTime.UtcNow);

        var cachedItem = await _cacheCollection.Find(filter).FirstOrDefaultAsync(ct);

        if (cachedItem is null)
            return default;

        return JsonSerializer.Deserialize<T>(cachedItem.Data);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan timeToLive, CancellationToken ct = default)
    {
        var jsonData = JsonSerializer.Serialize(value);
        var cacheEntry = new GameCacheModel
        {
            Id = key,
            Data = jsonData,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.Add(timeToLive)
        };

        await _cacheCollection.ReplaceOneAsync(
            x => x.Id == key,
            cacheEntry,
            new ReplaceOptions { IsUpsert = true },
            ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        await _cacheCollection.DeleteOneAsync(x => x.Id == key, ct);
    }
}
