using Posts.Application.Abstractions;
using Posts.Domain;
using SharedKernal.Messaging.Commands;
using SharedKernal.Results;

namespace Posts.Application.Posts.Commands.CreatePost;

internal sealed class CreatePostCommandHandler(IPostRepository repository)
    : ICommandHandler<CreatePostCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var result = Post.Create(
            request.AuthorId,
            request.MediaAssetId,
            request.Title,
            request.Description,
            request.CategoryId,
            request.MediaUrl,
            request.AltText);

        if (result.IsFailure)
            return result.Error;

        var post = result.Value;

        foreach (var tagName in request.Tags ?? [])
        {
            var tagResult = PostTag.Create(tagName);
            if (tagResult.IsFailure)
                return tagResult.Error;

            post.AddTag(tagResult.Value);
        }

        await repository.AddAsync(post, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return post.Id;
    }
}
