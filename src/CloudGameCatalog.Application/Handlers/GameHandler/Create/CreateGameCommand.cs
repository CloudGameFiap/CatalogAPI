using CloudGameCatalog.Domain.Handlers;

namespace CloudGameCatalog.Application.Handlers.GameHandler.Create;

public sealed class CreateGameCommand : ICommand
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public string Genre { get; set; }
    public DateTime ReleaseDate { get; set; }
}

