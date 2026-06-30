namespace CloudGameCatalog.Domain.Entities;

public class UserGame
{
    public Guid Id { get; set; }
    public Guid IdUser { get; set; }
    public Guid IdGame { get; set; }
}
