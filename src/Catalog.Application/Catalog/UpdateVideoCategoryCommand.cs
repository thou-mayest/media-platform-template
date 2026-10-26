using Catalog.Application.Abstractions;
using SharedKernal.Messaging;

namespace Catalog.Application.Catalog;

public sealed record UpdateVideoCategoryCommand(Guid VideoId, Guid CategoryId) : ICommand<VideoCardDto?>;

internal sealed class UpdateVideoCategoryCommandHandler(ICatalogRepository repository)
    : ICommandHandler<UpdateVideoCategoryCommand, VideoCardDto?>
{
    public async Task<VideoCardDto?> Handle(UpdateVideoCategoryCommand request, CancellationToken cancellationToken)
    {
        var video = await repository.GetVideoByIdAsync(request.VideoId, cancellationToken);
        if (video is null)
        {
            return null;
        }

        var category = await repository.GetCategoryByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
        {
            throw new InvalidOperationException($"Category with id '{request.CategoryId}' does not exist.");
        }

        video.ChangeCategory(category.Id);
        repository.UpdateVideo(video);
        await repository.SaveChangesAsync(cancellationToken);

        return video.ToVideoCardDto(video.Creator, category);
    }
}

