using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CloudGameCatalog.Infrastructure.MongoDb.Models;

public class GameCacheModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    public string Data { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("expiresAt")]
    public DateTime ExpiresAt { get; set; }
}