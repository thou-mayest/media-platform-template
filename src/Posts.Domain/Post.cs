using SharedKernal.Entities;
using SharedKernal.Results;

namespace Posts.Domain;

public sealed class Post : AggregateRoot
{
    private readonly List<PostTag> _tags = [];

    private Post(
        Guid id,
        Guid authorId,
        Guid mediaAssetId,
        string title,
        string description,
        Guid categoryId,
        string mediaUrl,
        string altText) : base(id)
    {
        AuthorId = authorId;
        MediaAssetId = mediaAssetId;
        Title = title;
        Description = description;
        CategoryId = categoryId;
        MediaUrl = mediaUrl;
        AltText = altText;
        Status = PostStatus.Draft;
    }

    private Post()
    {
        Title = null!;
        Description = null!;
        MediaUrl = null!;
        AltText = null!;
    }

    public Guid AuthorId { get; private set; }
    public Guid MediaAssetId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }
    public string MediaUrl { get; private set; }
    public string AltText { get; private set; }
    public PostStatus Status { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public long ViewCount { get; private set; }
    public IReadOnlyCollection<PostTag> Tags => _tags.AsReadOnly();

    public static Result<Post> Create(
        Guid authorId,
        Guid mediaAssetId,
        string? title,
        string? description,
        Guid? categoryId,
        string? mediaUrl,
        string? altText)
    {
        if (authorId == Guid.Empty)
            return Error.Validation("Post.AuthorRequired", "An author is required.");
        if (mediaAssetId == Guid.Empty)
            return Error.Validation("Post.MediaRequired", "A media asset is required.");
        if (string.IsNullOrWhiteSpace(title) || title.Trim().Length > 200)
            return Error.Validation("Post.TitleInvalid", "Title is required and cannot exceed 200 characters.");
        if (string.IsNullOrWhiteSpace(description) || description.Trim().Length > 2_000)
            return Error.Validation("Post.DescriptionInvalid", "Description is required and cannot exceed 2000 characters.");
        if (string.IsNullOrWhiteSpace(mediaUrl) || mediaUrl.Trim().Length > 2_048)
            return Error.Validation("Post.MediaUrlInvalid", "Media URL is required and cannot exceed 2048 characters.");
        if (string.IsNullOrWhiteSpace(altText) || altText.Trim().Length > 500)
            return Error.Validation("Post.AltTextInvalid", "Alt text is required and cannot exceed 500 characters.");

        var resolvedCategoryId = categoryId is null || categoryId == Guid.Empty ? Category.DefaultId : categoryId.Value;

        return new Post(
            Guid.NewGuid(), authorId, mediaAssetId, title.Trim(), description.Trim(), resolvedCategoryId,
            mediaUrl.Trim(), altText.Trim());
    }

    public Result AddTag(PostTag tag)
    {
        if (_tags.All(existing => existing.NormalizedName != tag.NormalizedName))
            _tags.Add(tag);

        return Result.Success();
    }

    public Result Publish(DateTime utcNow)
    {
        if (Status == PostStatus.Published)
            return Result.Failure(Error.Conflict("Post.AlreadyPublished", "Post is already published."));

        Status = PostStatus.Published;
        PublishedAt = utcNow.Kind == DateTimeKind.Utc ? utcNow : utcNow.ToUniversalTime();
        UpdateDate = PublishedAt;
        return Result.Success();
    }

    public Result MarkFailed()
    {
        if (Status == PostStatus.Failed)
            return Result.Success();

        Status = PostStatus.Failed;
        UpdateDate = DateTime.UtcNow;
        return Result.Success();
    }
}
