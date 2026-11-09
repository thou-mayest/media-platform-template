using SharedKernal.Entities;
using SharedKernal.Results;

namespace Posts.Domain;

public sealed class Category : Entity
{
    public static readonly Guid DefaultId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public const string DefaultName = "Uncategorized";

    private Category(Guid id, string name, string normalizedName) : base(id)
    {
        Name = name;
        NormalizedName = normalizedName;
    }

    private Category()
    {
        Name = null!;
        NormalizedName = null!;
    }

    public string Name { get; private set; }
    public string NormalizedName { get; private set; }
    public ICollection<Post> Posts { get; private set; } = new List<Post>();

    public static Result<Category> Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length is 0 or > 100)
            return Error.Validation("Category.NameInvalid", "Category name is required and cannot exceed 100 characters.");

        var cleaned = name.Trim();
        return new Category(Guid.NewGuid(), cleaned, Normalize(cleaned));
    }

    public static Category CreateDefault() => new(DefaultId, DefaultName, Normalize(DefaultName));

    public static string Normalize(string value) => value.Trim().ToLowerInvariant();
}
