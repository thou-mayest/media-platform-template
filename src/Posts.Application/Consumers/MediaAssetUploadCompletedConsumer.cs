using MassTransit;
using MediatR;
using Posts.Application.Posts.Commands.PublishPostForMedia;
using SharedKernal.Results;
using Storage.Contracts.IntegrationEvents;

namespace Posts.Application.Consumers;

internal sealed class MediaAssetUploadCompletedConsumer(ISender sender) : IConsumer<MediaAssetUploadCompletedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<MediaAssetUploadCompletedIntegrationEvent> context)
    {
        var result = await sender.Send(
            new PublishPostForMediaCommand(context.Message.MediaAssetId),
            context.CancellationToken);

        if (result.IsFailure && result.Error.Type != ErrorType.NotFound)
        {
            throw new InvalidOperationException(
                $"Failed to publish post for media asset {context.Message.MediaAssetId}: {result.Error.Message}");
        }
    }
}
