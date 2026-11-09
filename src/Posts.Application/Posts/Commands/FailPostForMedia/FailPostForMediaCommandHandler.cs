using Posts.Application.Abstractions;
using SharedKernal.Messaging.Commands;
using SharedKernal.Results;

namespace Posts.Application.Posts.Commands.FailPostForMedia;

internal sealed class FailPostForMediaCommandHandler(IPostRepository repository)
    : ICommandHandler<FailPostForMediaCommand, Result>
{
    public async Task<Result> Handle(FailPostForMediaCommand request, CancellationToken cancellationToken)
    {
        var post = await repository.GetByMediaAssetIdAsync(request.MediaAssetId, cancellationToken);

        if (post is null)
            return Error.NotFound(ErrorCodes.NotFound, $"Post for media asset {request.MediaAssetId} not found.");

        var result = post.MarkFailed();
        if (result.IsFailure)
            return result.Error;

        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
