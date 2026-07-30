using System.Text.RegularExpressions;
using SharedKernal.Results;
using SharedKernal.ValueObjects;

namespace Users.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled);

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    internal static Result<Email> Create(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Error.Validation("Email.Empty", "Email cannot be empty.");

        email = email.Trim();

        if (email.Length > 256)
            return Error.Validation("Email.TooLong", "Email must not exceed 256 characters.");

        if (!EmailRegex.IsMatch(email))
            return Error.Validation("Email.InvalidFormat", "Email format is invalid.");

        return new Email(email.ToLowerInvariant());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}