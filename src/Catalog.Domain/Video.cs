using SharedKernal.Entities;

namespace Catalog.Domain;

public sealed class Video : BaseEntity
{
    private Video()
    {
    }

    public Video(
        Guid creatorId,
        Guid categoryId,
        string title,
        string slug,
        int year,
        string medium,
        string size,
        double aspectRatio,
        string background,
        int displayOrder)
    {
        Id = Guid.NewGuid();
        CreatorId = creatorId;
        CategoryId = categoryId;
        Title = title;
        Slug = slug;
        Year = year;
        Medium = medium;
        Size = size;
        AspectRatio = aspectRatio;
        Background = background;
        DisplayOrder = displayOrder;
    }

    public Guid Id { get; private set; }

    public Guid CreatorId { get; private set; }

    public Creator Creator { get; private set; } = null!;

    public Guid CategoryId { get; private set; }

    public Category Category { get; private set; } = null!;

    public string Title { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public int Year { get; private set; }

    public string Medium { get; private set; } = string.Empty;

    public string Size { get; private set; } = string.Empty;

    public double AspectRatio { get; private set; }

    public string Background { get; private set; } = string.Empty;

    public int DisplayOrder { get; private set; }

    public ICollection<VideoTag> VideoTags { get; private set; } = [];

    public void ChangeCategory(Guid categoryId)
    {
        CategoryId = categoryId;
        UpdateDate = DateTime.UtcNow;
    }

    public void AddTag(Guid tagId)
    {
        if (VideoTags.All(vt => vt.TagId != tagId))
        {
            VideoTags.Add(new VideoTag(Id, tagId));
            UpdateDate = DateTime.UtcNow;
        }
    }

    public void RemoveTag(Guid tagId)
    {
        var tag = VideoTags.FirstOrDefault(vt => vt.TagId == tagId);
        if (tag is not null)
        {
            VideoTags.Remove(tag);
            UpdateDate = DateTime.UtcNow;
        }
    }
}
