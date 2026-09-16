using System.Security.Cryptography;
using Users.Domain.Abstractions;

namespace Users.Infrastracture.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 350000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

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
        var parts = hashedPassword.Split('.', 2);
        if (parts.Length != 2)
            return false;

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

    public void PerformFakeVerification()
    {
        byte[] salt = new byte[SaltSize]; // all zeros
        byte[] dummyHash = Rfc2898DeriveBytes.Pbkdf2(
            "AN_UNGUESSABLE_DUMMY_PASSWORD",
            salt,
            Iterations,
            Algorithm,
            HashSize);

        // Compare against an arbitrary byte array of the same length to run FixedTimeEquals.
        // The result is discarded; we only care about the time taken.
        byte[] fakeExpectedHash = new byte[HashSize];
        _ = CryptographicOperations.FixedTimeEquals(dummyHash, fakeExpectedHash);
    }
}