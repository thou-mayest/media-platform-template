using Microsoft.AspNetCore.Identity;
using Users.Application.Abstractions;

namespace Users.Infrastracture.Security;

internal sealed class AspNetPasswordHashingService : IPasswordHashingService
{
    private readonly PasswordHasher<string> _hasher = new();
    private readonly string _dummyHash;

    public AspNetPasswordHashingService()
    {
        _dummyHash = _hasher.HashPassword(string.Empty, "dummy-password-never-used");
    }

    public string Hash(string password) => _hasher.HashPassword(string.Empty, password);

    public bool Verify(string passwordHash, string password)
    {
        if (passwordHash == Users.Domain.User.InvalidatedPasswordHash)
        {
            return false;
        }

        try
        {
            return _hasher.VerifyHashedPassword(string.Empty, passwordHash, password) !=
                   PasswordVerificationResult.Failed;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public void VerifyDummy(string password) =>
        _hasher.VerifyHashedPassword(string.Empty, _dummyHash, password);
}
