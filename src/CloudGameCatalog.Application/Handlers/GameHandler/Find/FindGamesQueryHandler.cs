using CloudGameCatalog.Domain.Commom;
using CloudGameCatalog.Domain.Handlers;
using CloudGameCatalog.Domain.Interfaces;

namespace CloudGameCatalog.Application.Handlers.GameHandler.Find;

public sealed class FindGamesQueryHandler(IGameReadOnlyRepository GameReadOnlyRepository) : IHandler<FindGamesQuery, Pagination<FindGamesQueryResponse>>
{
    public async Task<Result<Pagination<FindGamesQueryResponse>>> HandleAsync(FindGamesQuery request, CancellationToken cancellationToken)
    {
        var games = await GameReadOnlyRepository.FindAsync(request.Parameters);

        var gamesResponse = games.Items.Select(s => new FindGamesQueryResponse(s.Id, s.Name, s.Active)).ToList();

        return Result<Pagination<FindGamesQueryResponse>>.Success(new Pagination<FindGamesQueryResponse>(gamesResponse, games.Count));
    }
}
