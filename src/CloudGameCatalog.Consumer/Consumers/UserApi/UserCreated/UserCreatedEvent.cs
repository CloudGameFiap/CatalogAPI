namespace CloudGameCatalog.Consumer.Consumers.UserApi.UserCreated;

public record UserCreatedEvent(int Id, string Name, string Email, DateTime BirthDate, bool Active, DateTime CreatedAt, bool IsAdmin);