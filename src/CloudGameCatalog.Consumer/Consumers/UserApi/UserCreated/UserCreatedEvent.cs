namespace CloudGameCatalog.Consumer.Consumers.UserApi.UserCreated;

public record UserCreatedEvent(string Name, string Email, DateTime BirthDate, bool Active, DateTime? UpdateAt, bool IsAdmin);