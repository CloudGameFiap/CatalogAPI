using CloudGameCatalog.Domain.Handlers;

namespace CloudGameCatalog.Application.Handlers.GameHandler.GetById;

public readonly record struct GetGameByIdResponse(int Id,
string Name,
string Description,
string? ImageUrl,
decimal Price,
string Genre,
DateTime ReleaseDate,
bool Active) : IResponse;


