namespace CloudGameCatalog.Application.Models.Search;

public sealed record GameSearchHit(GameSearchDocument Game, double? Score);

public sealed record GameSearchResult(long Total, int Page, int PageSize, IReadOnlyList<GameSearchHit> Items);
