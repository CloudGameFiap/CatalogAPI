using CloudGameCatalog.Domain.Commom;
using CloudGameCatalog.Domain.Handlers;
using CloudGameCatalog.Domain.Interfaces;

namespace CloudGameCatalog.Application.Handlers.GameHandler.GetById;

public sealed class GetGameByIdQueryHandler(IGameReadOnlyRepository gameReadOnlyRepository) : IHandler<GetGameByIdQuery, GetGameByIdQueryResponse>
{
    public async Task<Result<GetGameByIdQueryResponse>> HandleAsync(GetGameByIdQuery request, CancellationToken cancellationToken)
    {
        var game = await gameReadOnlyRepository.GetByIdAsync(request.Id);
        if (game is null)
            return Result<GetGameByIdQueryResponse>.Failure([new("NotFound", "Não foi encontrado jogo com id passado.")]);

        return Result<GetGameByIdQueryResponse>.Success(new GetGameByIdQueryResponse(game.Id, game.Name, game.Description, game.ImageUrl, game.Price, game.Genre, game.ReleaseDate, game.Active));
    }
}
