using CloudGameCatalog.Application.Interfaces;
using CloudGameCatalog.Application.Models.Search;
using CloudGameCatalog.Domain.Commom;
using CloudGameCatalog.Domain.Handlers;

namespace CloudGameCatalog.Application.Handlers.GameHandler.Search;

public sealed class SearchGamesQueryHandler(IGameSearchService gameSearchService)
    : IHandler<SearchGamesQuery, Pagination<SearchGamesQueryResponse>>
{
    public async Task<Result<Pagination<SearchGamesQueryResponse>>> HandleAsync(SearchGamesQuery request, CancellationToken cancellationToken)
    {
        var games = await gameSearchService.SearchAsync(new GameSearchRequest(
            request.Parameters.Text,
            request.Parameters.Genre,
            request.Parameters.MinPrice,
            request.Parameters.MaxPrice,
            request.Parameters.PageNumber ?? 1,
            request.Parameters.PageSize ?? 10), cancellationToken);

        var gamesResponse = games.Items
            .Select(s => new SearchGamesQueryResponse(s.Game.Id, s.Game.Name, s.Game.Description, s.Game.Price, s.Game.Genre))
            .ToList();
        var paginationResult = new Pagination<SearchGamesQueryResponse>(gamesResponse, (int)games.Total);

        return Result<Pagination<SearchGamesQueryResponse>>.Success(paginationResult);
    }
}
