using MassTransit;
using Posts.Application.Consumers;

namespace Posts.Application;

public static class MessageBusRegistration
{
    public static IBusRegistrationConfigurator AddPostsConsumers(
        this IBusRegistrationConfigurator bus)
    {
        bus.AddConsumer<MediaAssetUploadCompletedConsumer>();
        bus.AddConsumer<MediaAssetUploadFailedConsumer>();

        return bus;
    }
}
