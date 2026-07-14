using CloudGameCatalog.Domain.Handlers;

namespace CloudGameCatalog.Application.Handlers.UserGameHandler.GetGamesByUserId
{
    public sealed class GetGamesByUserIdQuery : ICommand
    {
        public int UserId { get; set; }
    }
}
