using System.Security.Cryptography;
using Konscious.Security.Cryptography;
using Users.Domain.Abstractions;

namespace Users.Infrastracture.Security;

public sealed class Argon2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int DegreeOfParallelism = 4;
    private const int Iterations = 4;
    private const int MemorySizeKb = 65536; // 64 MB

    public string Hash(string plainTextPassword)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = ComputeHash(plainTextPassword, salt);

        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verify(string plainTextPassword, string hashedPassword)
    {
        var parts = hashedPassword.Split('.', 2);
        if (parts.Length != 2)
            return false;

        var salt = Convert.FromBase64String(parts[0]);
        var expectedHash = Convert.FromBase64String(parts[1]);

        var actualHash = ComputeHash(plainTextPassword, salt);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    private static byte[] ComputeHash(string plainTextPassword, byte[] salt)
    {
        using var argon2 = new Argon2id(System.Text.Encoding.UTF8.GetBytes(plainTextPassword))
        {
            Salt = salt,
            DegreeOfParallelism = DegreeOfParallelism,
            Iterations = Iterations,
            MemorySize = MemorySizeKb
        };

        return argon2.GetBytes(HashSize);
    }
}