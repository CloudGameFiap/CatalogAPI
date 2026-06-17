using CloudGameCatalog.Domain.Handlers;

namespace CloudGameCatalog.Application.Handlers.GameHandler.Create;

public record CreateGameCommandResponse(int Id, string Nome, bool Active) : IResponse;
