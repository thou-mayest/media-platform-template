using MassTransit;
using MediatR;
using Posts.Application.Posts.Commands.FailPostForMedia;
using SharedKernal.Results;
using Storage.Contracts.IntegrationEvents;

namespace Posts.Application.Consumers;

internal sealed class MediaAssetUploadFailedConsumer(ISender sender) : IConsumer<MediaAssetUploadFailedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<MediaAssetUploadFailedIntegrationEvent> context)
    {
        var result = await sender.Send(
            new FailPostForMediaCommand(context.Message.MediaAssetId),
            context.CancellationToken);

        if (result.IsFailure && result.Error.Type != ErrorType.NotFound)
        {
            throw new InvalidOperationException(
                $"Failed to mark post as failed for media asset {context.Message.MediaAssetId}: {result.Error.Message}");
        }
    }
}
