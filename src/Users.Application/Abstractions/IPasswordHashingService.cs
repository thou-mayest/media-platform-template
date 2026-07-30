namespace Users.Application.Abstractions;

internal interface IPasswordHashingService
{
    string Hash(string password);

    bool Verify(string passwordHash, string password);

    void VerifyDummy(string password);
}
