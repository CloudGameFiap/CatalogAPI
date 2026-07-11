using CloudGameCatalog.Domain.Commom;
using CloudGameCatalog.Domain.Commom.Events;
using CloudGameCatalog.Domain.Entities;
using CloudGameCatalog.Domain.Handlers;
using CloudGameCatalog.Domain.Interfaces;
using MassTransit;

namespace CloudGameCatalog.Application.Handlers.UserGameHandler.AddGame
{
    public sealed class AddGameCommandHandler(
        IUserGameWriteOnlyRepository userGameWriteOnlyRepository,
    IUserGameReadOnlyRepository userGameReadOnlyRepository,
    IPublishEndpoint publishEndpoint,
    IUnitOfWork unitOfWork) : IHandler<AddGameCommand, AddGameCommandResponse>
    {
        public async Task<Result<AddGameCommandResponse>> HandleAsync(
            AddGameCommand command,
            CancellationToken cancellationToken)
        {
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
