using CloudGameCatalog.Domain.Commom;
using CloudGameCatalog.Domain.Handlers;
using CloudGameCatalog.Domain.Interfaces;

namespace CloudGameCatalog.Application.Handlers.GameHandler.Find;

public sealed class FindGamesQueryHandler(
    IGameReadOnlyRepository GameReadOnlyRepository,
    ICacheService cacheService) : IHandler<FindGamesQuery, Pagination<FindGamesQueryResponse>>
{
    private const string CollectionGames = "Games";

    public async Task<Result<Pagination<FindGamesQueryResponse>>> HandleAsync(FindGamesQuery request, CancellationToken cancellationToken)
    {
        if (request.Parameters.PageNumber is null)
            request.Parameters.PageNumber = 1;

        if (request.Parameters.PageSize is null)
            request.Parameters.PageSize = 10;

        var searchPattern = string.IsNullOrWhiteSpace(request.Parameters.Name) ? "all" : request.Parameters.Name;
        var cacheKey = $"games:page:{request.Parameters.PageNumber}:size:{request.Parameters.PageSize}:search:{searchPattern}";

        var cachedPagination = await cacheService.GetAsync<Pagination<FindGamesQueryResponse>>(CollectionGames, cacheKey, cancellationToken);
        if (cachedPagination != null)
        {
            return Result<Pagination<FindGamesQueryResponse>>.Success(cachedPagination);
        }

        var games = await GameReadOnlyRepository.FindAsync(request.Parameters);

        var gamesResponse = games.Items.Select(s => new FindGamesQueryResponse(s.Id, s.Name, s.Active)).ToList();
        var paginationResult = new Pagination<FindGamesQueryResponse>(gamesResponse, games.Count);

        if (gamesResponse.Count > 0)
        {
            await cacheService.SetAsync(
                CollectionGames,
                cacheKey,
                paginationResult,
                TimeSpan.FromSeconds(30),
                cancellationToken);
        }

        return Result<Pagination<FindGamesQueryResponse>>.Success(paginationResult);
    }
}