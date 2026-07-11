using CloudGameCatalog.Domain.Handlers;

namespace CloudGameCatalog.Application.Handlers.UserGameHandler.AddGame
{
    public record AddGameCommandResponse(int Id) : IResponse;
}
