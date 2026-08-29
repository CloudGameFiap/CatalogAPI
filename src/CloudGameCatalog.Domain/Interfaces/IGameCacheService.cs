namespace CloudGameCatalog.Domain.Interfaces;

public interface IGameCacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TimeSpan timeToLive, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
}