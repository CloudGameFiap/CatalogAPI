using CloudGameCatalog.Domain.Commom;
using CloudGameCatalog.Domain.Commom.Events;
using CloudGameCatalog.Domain.Entities;
using CloudGameCatalog.Domain.Handlers;
using CloudGameCatalog.Domain.Interfaces;
using MassTransit;

namespace CloudGameCatalog.Application.Handlers.GameHandler.Create;

public sealed class CreateGameCommandHandler(
    IGameWriteOnlyRepository gameWriteOnlyRepository,
    IUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint) : IHandler<CreateGameCommand, CreateGameCommandResponse>
{
    public async Task<Result<CreateGameCommandResponse>> HandleAsync(
        CreateGameCommand command,
        CancellationToken cancellationToken)
    {
        Game newGame = new(command.Name, command.Description, command.ImageUrl, command.Price, command.Genre, command.ReleaseDate);

        await gameWriteOnlyRepository.AddAsync(newGame);

        await unitOfWork.SaveChangesAsync();

        await publishEndpoint.Publish(new GameIndexRequestedEvent(newGame.Id), cancellationToken);

        return Result<CreateGameCommandResponse>.Success(new CreateGameCommandResponse(newGame.Id, newGame.Name, newGame.Active));
    }
}
