using CloudGameCatalog.Domain.Handlers;

namespace CloudGameCatalog.Application.Handlers.GameHandler.Update;

public record UpdateGameCommandResponse(int Id, string Nome, bool Active) : IResponse;
