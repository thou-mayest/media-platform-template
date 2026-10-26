using SharedKernal.Entities;

namespace Catalog.Domain;

public sealed class Category : BaseEntity
{
    public const string DefaultSlug = "uncategorized";
    public const string DefaultName = "Uncategorized";

    private Category()
    {
    }

    public Category(string name, string slug)
    {
        Id = Guid.NewGuid();
        Name = name;
        Slug = slug;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public ICollection<Video> Videos { get; private set; } = [];

    public void Update(string name, string slug)
    {
        Name = name;
        Slug = slug;
        UpdateDate = DateTime.UtcNow;
    }
}
