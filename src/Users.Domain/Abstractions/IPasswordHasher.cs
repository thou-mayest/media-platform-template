namespace Users.Domain.Abstractions;

public interface IPasswordHasher
{
    string Hash(string plainTextPassword);
    bool Verify(string plainTextPassword, string hashedPassword);
    void VerifyDummy(string plainTextPassword);
}
