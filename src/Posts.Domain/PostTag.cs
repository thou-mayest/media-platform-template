using SharedKernal.Results;

namespace Posts.Domain;

public sealed class PostTag
{
    private PostTag(string name, string normalizedName)
    {
        Name = name;
        NormalizedName = normalizedName;
    }

    private PostTag()
    {
        Name = null!;
        NormalizedName = null!;
    }

    public string Name { get; private set; }
    public string NormalizedName { get; private set; }

    public static Result<PostTag> Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().TrimStart('#').Length is 0 or > 50)
            return Error.Validation("Tag.NameInvalid", "Tag name is required and cannot exceed 50 characters.");

        var cleaned = name.Trim().TrimStart('#');
        return new PostTag(cleaned, Normalize(cleaned));
    }

    public static string Normalize(string value) => value.Trim().TrimStart('#').ToLowerInvariant();
}
