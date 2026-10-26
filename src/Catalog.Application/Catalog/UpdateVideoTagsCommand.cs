using Catalog.Application.Abstractions;
using SharedKernal.Messaging;

namespace Catalog.Application.Catalog;

public sealed record UpdateVideoTagsCommand(Guid VideoId, IReadOnlyList<Guid> TagIds) : ICommand<VideoCardDto?>;

internal sealed class UpdateVideoTagsCommandHandler(ICatalogRepository repository)
    : ICommandHandler<UpdateVideoTagsCommand, VideoCardDto?>
{
    public async Task<VideoCardDto?> Handle(UpdateVideoTagsCommand request, CancellationToken cancellationToken)
    {
        var video = await repository.GetVideoByIdAsync(request.VideoId, cancellationToken);
        if (video is null)
        {
            return null;
        }

        var targetTagIds = request.TagIds.Distinct().ToList();
        foreach (var tagId in targetTagIds)
        {
            var tag = await repository.GetTagByIdAsync(tagId, cancellationToken);
            if (tag is null)
            {
                throw new InvalidOperationException($"Tag with id '{tagId}' does not exist.");
            }
        }

        var currentTagIds = video.VideoTags.Select(vt => vt.TagId).ToList();
        foreach (var currentTagId in currentTagIds)
        {
            if (!targetTagIds.Contains(currentTagId))
            {
                video.RemoveTag(currentTagId);
            }
        }

        foreach (var tagId in targetTagIds)
        {
            video.AddTag(tagId);
        }

        repository.UpdateVideo(video);
        await repository.SaveChangesAsync(cancellationToken);

        var updated = await repository.GetVideoByIdAsync(video.Id, cancellationToken);
        return updated?.ToVideoCardDto();
    }
}

