using CloudGameCatalog.Application.Interfaces;
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
        IUnitOfWork unitOfWork,
        ICacheService cacheService) : IHandler<AddGameCommand, AddGameCommandResponse>
    {
        private const string CollectionUsers = "Users";
        private const string CollectionGames = "Games";
        private const string CollectionUserGames = "UserGames";

        public async Task<Result<AddGameCommandResponse>> HandleAsync(
            AddGameCommand command,
            CancellationToken cancellationToken)
        {
            var user = await GetUserWithCacheAsync(command.UserId);

                if (user is null)
                {
                    return Result<AddGameCommandResponse>.Failure([new Error("UserNotFound", "User not found, contact the support.")]);
                }

                var userToCache = new AddGameCommandResponse(user.Id);

                await cacheService.SetAsync(CollectionUsers, cacheUserKey, userToCache, TimeSpan.FromMinutes(30), cancellationToken);
            }

            var cacheGameKey = $"game:{command.GameId}";

            var cachedGame = await cacheService.GetAsync<FindGamesQueryResponse>(CollectionGames, cacheGameKey, cancellationToken);

            if(cachedGame == null)
            {
                var game = await gameReadOnlyRepository.GetByIdAsync(command.GameId);

                if (game is null)
                {
                    return Result<AddGameCommandResponse>.Failure([new Error("GameNotFound", "Game not found, contact the support.")]);
                }

                var gameToCache = new FindGamesQueryResponse(game.Id, game.Name, true);

                await cacheService.SetAsync(CollectionGames, cacheGameKey, gameToCache,TimeSpan.FromMinutes(30), cancellationToken);
            }

            var userGameKey = $"usergame:{command.UserId}:{command.GameId}";

            var cachedUserGame = await cacheService.GetAsync<UserGame>(CollectionUserGames, userGameKey, cancellationToken);

            if (cachedUserGame != null)
            {
                return Result<AddGameCommandResponse>.Failure([new Error("UserHasGame", "User already has this game.")]);
            }

            var userHasGame = await userGameReadOnlyRepository.GetByUserIdAndGameIdAsync(command.UserId, command.GameId);

            if (userHasGame is not null)
            {
                await cacheService.SetAsync(CollectionUserGames, userGameKey, userHasGame, TimeSpan.FromMinutes(30), cancellationToken);

                return Result<AddGameCommandResponse>.Failure([new Error("UserHasGame", "User already has this game.")]);
            }

            UserGame userGame = new(command.UserId, command.GameId, command.Price);

            await userGameWriteOnlyRepository.AddAsync(userGame);

            await unitOfWork.SaveChangesAsync();

            await publishEndpoint.Publish(new OrderPlacedEvent(command.UserId, command.GameId, command.Price), cancellationToken);

            return Result<AddGameCommandResponse>.Success(new AddGameCommandResponse(userGame.Id));
        }

        private async Task<User?> GetUserWithCacheAsync(int userId)
        {
            var cacheKey = $"user:{userId}";

            var cached = await cacheService.GetAsync<User>(cacheKey);
            if (cached is not null)
                return cached;

            var user = await userReadOnlyRepository.GetByIdAsync(userId);

            if (user is not null)
                await cacheService.SetAsync(cacheKey, user, TimeSpan.FromMinutes(15));

            return user;
        }
    }
}
