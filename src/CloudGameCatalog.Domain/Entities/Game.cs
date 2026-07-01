namespace CloudGameCatalog.Domain.Entities;

public sealed class Game : Entity<int>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string? ImageUrl { get; private set; }
    public decimal Price { get; private set; }
    public string Genre { get; private set; }
    public DateTime ReleaseDate { get; private set; }

    public bool Active { get; private set; }

    public Game(string name, string description, string? imageUrl, decimal price, string genre, DateTime releaseDate)
    {
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        Price = price;
        Genre = genre;
        ReleaseDate = releaseDate;
        Active = true;
        Validate();
    }

    public void Update(string name, string description, string? imageUrl, decimal price, string genre, DateTime releaseDate)
    {
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        Price = price;
        Genre = genre;
        ReleaseDate = releaseDate;
        Validate();
    }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Game name cannot be empty or null.");

        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException("Game description cannot be empty or null.");

        if (string.IsNullOrWhiteSpace(ImageUrl))
            throw new ArgumentException("Game image URL cannot be empty or null.");

        if (string.IsNullOrWhiteSpace(Genre))
            throw new ArgumentException("Game genre cannot be empty or null.");

        if (Price < 0)
            throw new ArgumentException("Game price cannot be negative or empty.");
    }

    private Game() { }

}
