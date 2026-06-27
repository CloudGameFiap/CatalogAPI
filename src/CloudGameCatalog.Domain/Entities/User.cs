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

    private User() { }
}