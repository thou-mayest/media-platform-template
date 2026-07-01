using Users.Domain.Abstractions;

namespace Users.Domain.UnitTests;

public sealed class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string plainTextPassword) => $"hashed:{plainTextPassword}";

    public bool Verify(string plainTextPassword, string hashedPassword) =>
        hashedPassword == $"hashed:{plainTextPassword}";
}