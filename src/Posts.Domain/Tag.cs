using SharedKernal.Entities;
using SharedKernal.Results;

namespace Posts.Domain;

public sealed class Tag : Entity
{
    public const int MaxNameLength = 50;

    private Tag(Guid id, string name, string normalizedName) : base(id)
    {
        Name = name;
        NormalizedName = normalizedName;
        UsageCount = 0;
    }

    private Tag()
    {
        Name = null!;
        NormalizedName = null!;
    }

    /// <summary>Display form, taken from the first time the tag was used (e.g. "DotNet").</summary>
    public string Name { get; private set; }

    /// <summary>Unique lookup key (e.g. "dotnet").</summary>
    public string NormalizedName { get; private set; }

    /// <summary>Denormalized number of posts using this tag; maintained asynchronously.</summary>
    public int UsageCount { get; private set; }

    public static Result<Tag> Create(string? name)
    {
        var cleaned = Clean(name);
        if (cleaned.Length is 0 or > MaxNameLength)
            return Error.Validation("Tag.NameInvalid", $"Tag name is required and cannot exceed {MaxNameLength} characters.");

        return new Tag(Guid.NewGuid(), cleaned, Normalize(cleaned));
    }

    public static string Normalize(string value) => Clean(value).ToLowerInvariant();

    private static string Clean(string? value) => (value ?? string.Empty).Trim().TrimStart('#').Trim();
}
