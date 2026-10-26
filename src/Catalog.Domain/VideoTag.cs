namespace Catalog.Domain;

public sealed class VideoTag
{
    private VideoTag()
    {
    }

    public VideoTag(Guid videoId, Guid tagId)
    {
        VideoId = videoId;
        TagId = tagId;
    }

    public Guid VideoId { get; private set; }

    public Video Video { get; private set; } = null!;

    public Guid TagId { get; private set; }

    public Tag Tag { get; private set; } = null!;
}
