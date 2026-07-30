using System.Security.Cryptography;
using Users.Domain.Abstractions;

namespace Users.Infrastracture.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 350000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;
    private readonly string _dummyHash;

    public PasswordHasher()
    {
        _dummyHash = Hash("DummyPassword123");
    }

    public string Hash(string plainTextPassword)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            plainTextPassword,
            salt,
            Iterations,
            Algorithm,
            HashSize);

        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verify(string plainTextPassword, string hashedPassword)
    {
        try
        {
            var parts = hashedPassword.Split('.', 2);
            if (parts.Length != 2)
            {
                VerifyDummy(plainTextPassword);
                return false;
            }

            var salt = Convert.FromBase64String(parts[0]);
            var expectedHash = Convert.FromBase64String(parts[1]);

            var actualHash = Rfc2898DeriveBytes.Pbkdf2(
                plainTextPassword,
                salt,
                Iterations,
                Algorithm,
                HashSize);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        catch (FormatException)
        {
            VerifyDummy(plainTextPassword);
            return false;
        }
    }

    public void VerifyDummy(string plainTextPassword) => Verify(plainTextPassword, _dummyHash);
}
