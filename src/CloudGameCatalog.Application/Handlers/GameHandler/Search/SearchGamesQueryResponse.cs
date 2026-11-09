namespace CloudGameCatalog.Application.Handlers.GameHandler.Search;

public record SearchGamesQueryResponse(int Id, string Name, string Description, decimal Price, string Genre);
