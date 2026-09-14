using CloudGameCatalog.Infrastructure.MongoDb.Models;
using MongoDB.Driver;

namespace CloudGameCatalog.Infrastructure.MongoDb.Context;

public interface IMongoDbContext
{
    IMongoCollection<CacheModel> GetCollection(string name);
}