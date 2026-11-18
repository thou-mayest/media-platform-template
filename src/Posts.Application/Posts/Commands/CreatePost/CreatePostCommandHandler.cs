using Posts.Application.Abstractions;
using Posts.Domain;
using SharedKernal.Messaging.Commands;
using SharedKernal.Results;

namespace Posts.Application.Posts.Commands.CreatePost;

internal sealed class CreatePostCommandHandler(IPostRepository repository, ITagRepository tagRepository)
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

        var candidates = new List<Tag>();
        foreach (var tagName in request.Tags ?? [])
        {
            var tagResult = Tag.Create(tagName);
            if (tagResult.IsFailure)
                return tagResult.Error;

            candidates.Add(tagResult.Value);
        }

        var tags = await tagRepository.GetOrCreateAsync(candidates, cancellationToken);
        foreach (var tag in tags)
            post.AddTag(tag);

        await repository.AddAsync(post, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return post.Id;
    }
}
