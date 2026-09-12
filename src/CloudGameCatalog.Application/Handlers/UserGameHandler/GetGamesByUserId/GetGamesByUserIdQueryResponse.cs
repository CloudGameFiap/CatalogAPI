using CloudGameCatalog.Domain.Handlers;

namespace CloudGameCatalog.Application.Handlers.UserGameHandler.GetGamesByUserId
{
    public class GetGamesByUserIdQueryResponse : IResponse
    {
        public List<UserGameDTO> Games { get; set; }
    }

    public class UserGameDTO
    {
        public int GameId { get; set; }

        public int PaymentStatus { get; set; }

        public string GameDescription { get; set; }
    }
}
