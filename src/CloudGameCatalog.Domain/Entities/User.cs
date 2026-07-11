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

    public string Name { get; private set; }

    public string Email { get; private set; }

    public DateTime BirthDate { get; private set; }

    public bool Active { get; private set; }

    public DateTime? UpdateAt { get; private set; }

    public bool IsAdmin { get; private set; }    

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