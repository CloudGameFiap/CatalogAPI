using CloudGameCatalog.Domain.Commom;
using CloudGameCatalog.Domain.Handlers;
using CloudGameCatalog.Domain.Interfaces;

namespace CloudGameCatalog.Application.Handlers.UserGameHandler.GetGamesByUserId
{
    public class GetGamesByUserIdQueryHandler(IUserGameReadOnlyRepository userGameReadOnlyRepository, ICacheService cacheService) : IHandler<GetGamesByUserIdQuery, GetGamesByUserIdQueryResponse>
    {
        private const string CollectionUsers = "User";

        public async Task<Result<GetGamesByUserIdQueryResponse>> HandleAsync(GetGamesByUserIdQuery command, CancellationToken cancellationToken)
        {
            var cacheUserKey = $"user:{command.UserId}";

            var cachedUser = await cacheService.GetAsync<GetGamesByUserIdQueryResponse>(CollectionUsers, cacheUserKey, cancellationToken);

            if (cachedUser != null)
            {
                return Result<GetGamesByUserIdQueryResponse>.Success(cachedUser);
            }

            var myGames = await userGameReadOnlyRepository.GetByUserIdAsync(command.UserId);

            var gamesDto = (myGames.Select(s => new UserGameDTO() { GameId = s.GameId, PaymentStatus = (int)s.Status })).ToList();

            var gamesByUserId = new GetGamesByUserIdQueryResponse() { Games = gamesDto };

            await cacheService.SetAsync(CollectionUsers, cacheUserKey, gamesByUserId, TimeSpan.FromMinutes(5), cancellationToken);

            return Result<GetGamesByUserIdQueryResponse>.Success(gamesByUserId);
        }
    }
}
