namespace Posts.Domain;

/// <summary>Join row between a <see cref="Post"/> and a <see cref="Domain.Tag"/>.</summary>
public sealed class PostTag
{
    internal PostTag(Guid postId, Tag tag)
    {
        PostId = postId;
        TagId = tag.Id;
        Tag = tag;
    }

    private PostTag() { }

    public Guid PostId { get; private set; }
    public Guid TagId { get; private set; }
    public Tag? Tag { get; private set; }
}
