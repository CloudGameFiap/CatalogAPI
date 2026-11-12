namespace CloudGameCatalog.Application.Models.Search;

public sealed record GameSearchRequest(
    string? Query,
    string? Genre = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    int Page = 1,
    int PageSize = 10);
