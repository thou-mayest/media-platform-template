using SharedKernal.Entities;

namespace Catalog.Domain;

public sealed class Tag : BaseEntity
{
    public const string DefaultSlug = "untagged";
    public const string DefaultName = "Untagged";

    private Tag()
    {
    }

    public Tag(string name, string slug)
    {
        Id = Guid.NewGuid();
        Name = name;
        Slug = slug;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public ICollection<VideoTag> VideoTags { get; private set; } = [];

    public void Update(string name, string slug)
    {
        Name = name;
        Slug = slug;
        UpdateDate = DateTime.UtcNow;
    }
}
