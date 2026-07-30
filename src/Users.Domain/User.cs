using SharedKernal.Entities;
using Users.Common;

namespace Users.Domain;

public class User : BaseEntity
{
    public const string InvalidatedPasswordHash = "!PASSWORD-RESET-REQUIRED!";

    private User()
    {
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public Role Role { get; private set; }

    public User(string name, string email, string passwordHash, Role role)
    {
        Id = Guid.NewGuid();
        Name = name.Trim();
        Email = NormalizeEmail(email);
        PasswordHash = passwordHash;
        Role = role;
    }

    public void Update(string name, string email, string passwordHash, Role role)
    {
        Name = name.Trim();
        Email = NormalizeEmail(email);
        PasswordHash = passwordHash;
        Role = role;
        UpdateDate = DateTime.UtcNow;
        Version = Guid.NewGuid();
    }

    public static string NormalizeEmail(string email) => email.Trim().ToUpperInvariant();

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        UpdateDate = DateTime.UtcNow;
        Version = Guid.NewGuid();
    }
}
