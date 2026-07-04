namespace CloudGameCatalog.Domain.Entities;

public sealed class User : Entity<int>
{
    public User(string name, string email, DateTime birthDate, bool isAdmin)
    {
        Name = name;
        Email = email;
        BirthDate = birthDate;
        Active = true;
        IsAdmin = isAdmin;
    }

    public string Name { get; protected set; }

    public string Email { get; protected set; }

    public DateTime BirthDate { get; protected set; }

    public bool Active { get; protected set; }

    public DateTime? UpdateAt { get; protected set; }

    public bool IsAdmin { get; protected set; }    

    public static User Create(int Id, string Name, string Email, DateTime BirthDate, bool Active, DateTime CreatedAt, bool IsAdmin)
    {
        User user = new(Name, Email, BirthDate, IsAdmin)
        {
            Id = Id,
            Active = Active,
            CreatedAt = CreatedAt
        };

        return user;
    }

    private User() { }
}