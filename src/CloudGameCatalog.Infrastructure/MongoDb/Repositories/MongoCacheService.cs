using CloudGameCatalog.Domain.Interfaces;
using CloudGameCatalog.Infrastructure.MongoDb.Context;
using CloudGameCatalog.Infrastructure.MongoDb.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Text.Json;

namespace CloudGameCatalog.Infrastructure.MongoDb.Repositories;

public class MongoCacheService : ICacheService
{
    private readonly IMongoDbContext _context;

    public MongoCacheService(IMongoDbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetAsync<T>(string collectionName, string key, CancellationToken ct = default)
    {
        var collection = _context.GetCollection(collectionName);

        var filter = Builders<CacheModel>.Filter.Eq(x => x.Id, key) &
                     Builders<CacheModel>.Filter.Gt(x => x.ExpiresAt, DateTime.UtcNow);

        var cachedItem = await collection.Find(filter).FirstOrDefaultAsync(ct);

        if (cachedItem is null)
            return default;

        return JsonSerializer.Deserialize<T>(cachedItem.Data);
    }

    public async Task SetAsync<T>(string collectionName, string key, T value, TimeSpan timeToLive, CancellationToken ct = default)
    {
        var collection = _context.GetCollection(collectionName);
        var jsonData = JsonSerializer.Serialize(value);

        var cacheEntry = new CacheModel
        {
            Id = key,
            Data = jsonData,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.Add(timeToLive)
        };

        await collection.ReplaceOneAsync(
            x => x.Id == key,
            cacheEntry,
            new ReplaceOptions { IsUpsert = true },
            ct);
    }

    public async Task RemoveAsync(string collectionName, string key, CancellationToken ct = default)
    {
        var collection = _context.GetCollection(collectionName);
        await collection.DeleteOneAsync(x => x.Id == key, ct);
    }
}