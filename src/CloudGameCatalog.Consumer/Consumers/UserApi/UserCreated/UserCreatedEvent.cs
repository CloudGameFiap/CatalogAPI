namespace CloudGame.Domain.Events.User;

public class UserCreatedEvent
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsAdmin { get; set; }
}