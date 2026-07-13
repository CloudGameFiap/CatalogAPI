using CloudGameCatalog.Domain.Commom;
using CloudGameCatalog.Domain.Commom.Events;
using CloudGameCatalog.Domain.Entities;
using CloudGameCatalog.Domain.Handlers;
using CloudGameCatalog.Domain.Interfaces;
using MassTransit;

namespace CloudGameCatalog.Application.Handlers.UserGameHandler.AddGame
{
    public sealed class AddGameCommandHandler(
        IGameReadOnlyRepository gameReadOnlyRepository,
        IUserReadOnlyRepository userReadOnlyRepository,
        IUserGameWriteOnlyRepository userGameWriteOnlyRepository,
        IUserGameReadOnlyRepository userGameReadOnlyRepository,
        IPublishEndpoint publishEndpoint,
        IUnitOfWork unitOfWork) : IHandler<AddGameCommand, AddGameCommandResponse>
    {
        public async Task<Result<AddGameCommandResponse>> HandleAsync(
            AddGameCommand command,
            CancellationToken cancellationToken)
        {
            var user = await userReadOnlyRepository.GetByIdAsync(command.UserId);

            if (user is null)
            {
                return Result<AddGameCommandResponse>.Failure([new Error("UserNotFound", "User not found, contact the support.")]);
            }

            var game = await gameReadOnlyRepository.GetByIdAsync(command.GameId);

            if (game is null)
            {
                return Result<AddGameCommandResponse>.Failure([new Error("GameNotFound", "Game not found, contact the support.")]);
            }

            var userHasGame = await userGameReadOnlyRepository.GetByUserIdAndGameIdAsync(command.UserId, command.GameId);

            if (userHasGame is not null)
            {
                return Result<AddGameCommandResponse>.Failure([new Error("UserHasGame", "User already has this game.")]);
            }

            UserGame userGame = new(command.UserId, command.GameId, command.Price);

            await userGameWriteOnlyRepository.AddAsync(userGame);

            await unitOfWork.SaveChangesAsync();

            await publishEndpoint.Publish(new OrderPlacedEvent(command.UserId, command.GameId, command.Price), cancellationToken);

            return Result<AddGameCommandResponse>.Success(new AddGameCommandResponse(userGame.Id));
        }
    }
}
