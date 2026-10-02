using MassTransit;
using MediatR;
using Profiles.Application.ActorProfiles.Commands.CreateActorProfile;
using Users.Contracts.IntegrationEvents;

namespace Profiles.Application.Consumers;


internal sealed class UserCreatedConsumer(ISender sender) : IConsumer<UserCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<UserCreatedIntegrationEvent> context)
    {
        var result = await sender.Send(
            new CreateActorProfileCommand(context.Message.UserId, context.Message.Name),
            context.CancellationToken);

       
        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Could not provision a profile for user {context.Message.UserId}: " +
                string.Join("; ", result.Errors.Select(e => $"{e.Code} {e.Message}")));
        }
    }
}
