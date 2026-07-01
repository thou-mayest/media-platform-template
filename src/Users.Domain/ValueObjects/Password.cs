using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedKernal.Results;
using SharedKernal.ValueObjects;
using Users.Domain.Abstractions;

namespace Users.Domain.ValueObjects;

public sealed class Password : ValueObject
{
    private const int MinLength = 8;

    public string HashedValue { get; }

    private Password(string hashedValue)
    {
        HashedValue = hashedValue;
    }

    internal static Result<Password> Create(string? plainTextPassword, IPasswordHasher hasher)
    {
        if (string.IsNullOrWhiteSpace(plainTextPassword))
            return Result.Failure<Password>(Error.Validation("Password.Empty", "Password cannot be empty."));

        if (plainTextPassword.Length < MinLength)
            return Result.Failure<Password>(Error.Validation("Password.TooShort", $"Password must be at least {MinLength} characters long."));

        if (!plainTextPassword.Any(char.IsDigit))
            return Result.Failure<Password>(Error.Validation("Password.MissingDigit", "Password must contain at least one digit."));

        if (!plainTextPassword.Any(char.IsUpper))
            return Result.Failure<Password>(Error.Validation("Password.MissingUppercase", "Password must contain at least one uppercase letter."));

        var hashed = hasher.Hash(plainTextPassword);
        return Result.Success(new Password(hashed));
    }

    // Used when rehydrating from persistence — value is already hashed, skip strength checks
    public static Password FromHash(string hashedValue) => new(hashedValue);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return HashedValue;
    }
}
