using CloudGameCatalog.Domain.Commom;
using CloudGameCatalog.Domain.Handlers;
using CloudGameCatalog.Domain.Interfaces;

namespace CloudGameCatalog.Application.Handlers.GameHandler.GetById;

public sealed class GetGameByIdQueryHandler(IGameReadOnlyRepository gameReadOnlyRepository) : IHandler<GetGameByIdQuery, GetGameByIdResponse>
{
    public async Task<Result<GetGameByIdResponse>> HandleAsync(GetGameByIdQuery request, CancellationToken cancellationToken)
    {
        var game = await gameReadOnlyRepository.GetByIdAsync(request.Id);
        if (game is null)
            return Result<GetGameByIdResponse>.Failure([new("NotFound", "Não foi encontrado jogo com id passado.")]);

        return Result<GetGameByIdResponse>.Success(new GetGameByIdResponse(game.Id, game.Name, game.Description, game.ImageUrl, game.Price, game.Genre, game.ReleaseDate, game.Active));
    }
}
