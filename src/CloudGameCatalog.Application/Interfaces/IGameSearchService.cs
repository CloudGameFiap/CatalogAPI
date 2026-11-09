using CloudGameCatalog.Application.Models.Search;

namespace CloudGameCatalog.Application.Interfaces
{
    public interface IGameSearchService
    {
        /// <returns><c>true</c> quando o índice foi criado nesta chamada.</returns>
        Task<bool> EnsureIndexAsync(CancellationToken ct = default);
        Task IndexAsync(GameSearchDocument document, long version, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<int> BulkIndexAsync(IReadOnlyCollection<GameSearchDocument> documents, CancellationToken ct = default);
        Task<GameSearchResult> SearchAsync(GameSearchRequest request, CancellationToken ct = default);
    }
}
