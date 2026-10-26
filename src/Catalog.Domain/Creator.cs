using SharedKernal.Entities;

namespace Catalog.Domain;

public sealed class Creator : BaseEntity
{
    private Creator()
    {
    }

    public Creator(
        string name,
        string slug,
        string location,
        string disciplines,
        string bio,
        string accent,
        string initials,
        string since)
    {
        Id = Guid.NewGuid();
        Name = name;
        Slug = slug;
        Location = location;
        Disciplines = disciplines;
        Bio = bio;
        Accent = accent;
        Initials = initials;
        Since = since;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public string Location { get; private set; } = string.Empty;

    public string Disciplines { get; private set; } = string.Empty;

    public string Bio { get; private set; } = string.Empty;

    public string Accent { get; private set; } = string.Empty;

    public string Initials { get; private set; } = string.Empty;

    public string Since { get; private set; } = string.Empty;

    public ICollection<Video> Videos { get; private set; } = [];
}
