using MassTransit;
using Profiles.Application.Consumers;

namespace Profiles.Application;

public static class MessageBusRegistration
{
    public static IBusRegistrationConfigurator AddProfilesConsumers(
        this IBusRegistrationConfigurator bus)
    {
        bus.AddConsumer<UserCreatedConsumer>();
        bus.AddConsumer<UserDeletedConsumer>();

        return bus;
    }
}
