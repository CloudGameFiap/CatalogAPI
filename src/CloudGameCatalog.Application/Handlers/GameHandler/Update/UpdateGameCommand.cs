using CloudGameCatalog.Domain.Handlers;
using System.Text.Json.Serialization;

namespace CloudGameCatalog.Application.Handlers.GameHandler.Update;

public sealed class UpdateGameCommand : ICommand
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string? ImageUrl { get; set; }

    public decimal Price { get; set; }

    public string Genre { get; set; }

    public DateTime ReleaseDate { get; set; }
}
