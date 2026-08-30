namespace CloudGameCatalog.Domain.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string collectionName, string key, CancellationToken ct = default);
    Task SetAsync<T>(string collectionName, string key, T value, TimeSpan timeToLive, CancellationToken ct = default);
    Task RemoveAsync(string collectionName, string key, CancellationToken ct = default);
}