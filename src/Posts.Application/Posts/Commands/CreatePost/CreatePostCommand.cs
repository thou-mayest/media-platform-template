using SharedKernal.Messaging.Commands;
using SharedKernal.Results;

namespace Posts.Application.Posts.Commands.CreatePost;

public sealed record CreatePostCommand(
    Guid AuthorId,
    Guid MediaAssetId,
    string? Title,
    string? Description,
    Guid? CategoryId,
    string? MediaUrl,
    string? AltText,
    IReadOnlyList<string>? Tags) : ICommand<Result<Guid>>;
