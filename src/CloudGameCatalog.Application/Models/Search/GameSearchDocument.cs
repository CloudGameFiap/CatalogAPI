using CloudGameCatalog.Domain.Entities;

namespace CloudGameCatalog.Application.Models.Search;

/// <summary>
/// Documento desnormalizado do índice. É um modelo de leitura,
/// separado da entidade do EF Core — o SQL Server continua sendo a fonte da verdade.
/// </summary>
public sealed class GameSearchDocument
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public string Genre { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public bool Active { get; set; }
    public DateTime IndexedAt { get; set; }

    public static GameSearchDocument From(Game game) => new()
    {
        Id = game.Id,
        Name = game.Name,
        Description = game.Description,
        ImageUrl = game.ImageUrl,
        Price = game.Price,
        Genre = game.Genre,
        ReleaseDate = game.ReleaseDate,
        Active = game.Active,
        IndexedAt = DateTime.UtcNow
    };
}
