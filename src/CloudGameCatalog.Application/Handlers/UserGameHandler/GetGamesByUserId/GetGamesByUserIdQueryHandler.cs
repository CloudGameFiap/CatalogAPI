using CloudGameCatalog.Domain.Commom;
using CloudGameCatalog.Domain.Handlers;
using CloudGameCatalog.Domain.Interfaces;

namespace CloudGameCatalog.Application.Handlers.UserGameHandler.GetGamesByUserId
{
    public class GetGamesByUserIdQueryHandler(IUserGameReadOnlyRepository userGameReadOnlyRepository, ICacheService cacheService) : IHandler<GetGamesByUserIdQuery, GetGamesByUserIdQueryResponse>
    {
        private const string CollectionUsers = "Users";

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

            return Result<GetGamesByUserIdQueryResponse>.Success(new GetGamesByUserIdQueryResponse() { Games = gamesDto });
        }
    }
}
