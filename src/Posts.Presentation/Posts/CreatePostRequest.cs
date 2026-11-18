namespace Posts.Presentation.Posts;

public sealed record CreatePostRequest(
    Guid MediaAssetId,
    string? Title,
    string? Description,
    Guid? CategoryId,
    string? MediaUrl,
    string? AltText,
    IReadOnlyList<string>? Tags);
