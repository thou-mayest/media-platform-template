using SharedKernal.Entities;

namespace Users.Domain;

public class User : BaseEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

   public string Role { get; set; }

    public User(string name, string email, string password, string role)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Password = password;
        Role = role;
    }

    public void Update(string name, string email, string password, string role)
    {
        Name = name;
        Email = email;
        Password = password;
        Role = role;
        UpdateDate = DateTime.UtcNow;
    }
}
