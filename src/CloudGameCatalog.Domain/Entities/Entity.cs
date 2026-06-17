namespace CloudGameCatalog.Domain.Entities;

public abstract class Entity<TId>
{
    public TId Id { get; protected set; } = default!;

    public DateTime CreatedAt { get; protected set; }

    public Entity()
    {
        CreatedAt = DateTime.UtcNow;
    }
}
