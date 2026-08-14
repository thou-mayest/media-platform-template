using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using SharedKernal.Results;
using SharedKernal.ValueObjects;

namespace Profiles.Domain.ValueObjects;


public sealed class ProfileSlug : ValueObject
{
    public const int MinLength = 3;
    public const int MaxLength = 64;

    private static readonly Regex Shape =
        new("^[a-z0-9]+(-[a-z0-9]+)*$", RegexOptions.Compiled);

    private static readonly Regex NonAlphanumeric =
        new("[^a-z0-9]+", RegexOptions.Compiled);

   
    private static readonly HashSet<string> Reserved = new(StringComparer.Ordinal)
    {
        "admin", "api", "assets", "edit", "login", "logout", "me", "new",
        "robots", "search", "settings", "signup", "sitemap", "static",
    };

    public string Value { get; }

    private ProfileSlug(string value) => Value = value;

    internal static Result<ProfileSlug> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("ProfileSlug.Empty", "Slug cannot be empty.");

        var slug = value.Trim().ToLowerInvariant();

        if (slug.Length < MinLength)
            return Error.Validation("ProfileSlug.TooShort", $"Slug must be at least {MinLength} characters.");

        if (slug.Length > MaxLength)
            return Error.Validation("ProfileSlug.TooLong", $"Slug must not exceed {MaxLength} characters.");

        if (!Shape.IsMatch(slug))
            return Error.Validation(
                "ProfileSlug.InvalidFormat",
                "Slug may contain only lowercase letters, digits and single hyphens between them.");

        if (Reserved.Contains(slug))
            return Error.Validation("ProfileSlug.Reserved", $"'{slug}' is a reserved slug.");

        return new ProfileSlug(slug);
    }


    public static string Slugify(string displayName)
    {
        var normalised = displayName.Normalize(NormalizationForm.FormKD);

        var stripped = new StringBuilder(normalised.Length);
        foreach (var c in normalised)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                stripped.Append(c);
        }

        var slug = NonAlphanumeric.Replace(stripped.ToString().ToLowerInvariant(), "-");
        return slug.Trim('-');
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}