using Posts.Application.Abstractions;
using SharedKernal.Messaging.Commands;
using SharedKernal.Results;

namespace Posts.Application.Posts.Commands.PublishPostForMedia;

internal sealed class PublishPostForMediaCommandHandler(IPostRepository repository)
    : ICommandHandler<PublishPostForMediaCommand, Result>
{
    public async Task<Result> Handle(PublishPostForMediaCommand request, CancellationToken cancellationToken)
    {
        var post = await repository.GetByMediaAssetIdAsync(request.MediaAssetId, cancellationToken);

        if (post is null)
            return Error.NotFound(ErrorCodes.NotFound, $"Post for media asset {request.MediaAssetId} not found.");

        var result = post.Publish(DateTime.UtcNow);
        if (result.IsFailure)
            return result.Error;

        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
