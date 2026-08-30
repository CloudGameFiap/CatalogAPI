using CloudGameCatalog.Domain.Commom;
using CloudGameCatalog.Domain.Handlers;
using CloudGameCatalog.Domain.Interfaces;

namespace CloudGameCatalog.Application.Handlers.GameHandler.Update;

public sealed class UpdateGameCommandHandler(
    IGameWriteOnlyRepository gameWriteOnlyRepository,
    IGameReadOnlyRepository gameReadOnlyRepository,
    IUnitOfWork unitOfWork,
    ICacheService cacheService)
    : IHandler<UpdateGameCommand, UpdateGameCommandResponse>
{
    private const string CollectionGames = "Games";

    public async Task<Result<UpdateGameCommandResponse>> HandleAsync(
        UpdateGameCommand command,
        CancellationToken cancellationToken)
    {
        var gameToUpdate = await gameReadOnlyRepository.GetByIdAsync(command.Id);

        if (gameToUpdate is null)
            return Result<UpdateGameCommandResponse>.Failure([new Error("NotFound", "Jogo não encontrado")]);

        var cacheKey = $"game:{command.Id}";

        gameToUpdate.Update
        (
            command.Name,
            command.Description,
            command.ImageUrl,
            command.Price,
            command.Genre,
            command.ReleaseDate
        );

        await gameWriteOnlyRepository.UpdateAsync(gameToUpdate);

        await unitOfWork.SaveChangesAsync();

        await cacheService.RemoveAsync(CollectionGames, cacheKey, cancellationToken);

        return Result<UpdateGameCommandResponse>.Success(
            new UpdateGameCommandResponse(gameToUpdate.Id, gameToUpdate.Name, gameToUpdate.Active));
    }
}