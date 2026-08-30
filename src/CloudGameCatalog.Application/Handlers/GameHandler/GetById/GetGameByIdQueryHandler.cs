using CloudGameCatalog.Domain.Commom;
using CloudGameCatalog.Domain.Handlers;
using CloudGameCatalog.Domain.Interfaces;

namespace CloudGameCatalog.Application.Handlers.GameHandler.GetById;

public sealed class GetGameByIdQueryHandler(IGameReadOnlyRepository gameReadOnlyRepository, ICacheService cacheService) : IHandler<GetGameByIdQuery, GetGameByIdQueryResponse>
{
    private const string CollectionName = "Games";

    public async Task<Result<GetGameByIdQueryResponse>> HandleAsync(GetGameByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"game:{request.Id}";

        var cachedGame = await cacheService.GetAsync<GetGameByIdQueryResponse>(CollectionName, cacheKey, cancellationToken);
        if (cachedGame != null)
        {
            return Result<GetGameByIdQueryResponse>.Success(cachedGame);
        }

        var game = await gameReadOnlyRepository.GetByIdAsync(request.Id);
        if (game is null)
            return Result<GetGameByIdQueryResponse>.Failure([new("NotFound", "Não foi encontrado jogo com id passado.")]);

        var response = new GetGameByIdQueryResponse(
            game.Id,
            game.Name,
            game.Description,
            game.ImageUrl,
            game.Price,
            game.Genre,
            game.ReleaseDate,
            game.Active);

        await cacheService.SetAsync(CollectionName, cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return Result<GetGameByIdQueryResponse>.Success(response);
    }
}
