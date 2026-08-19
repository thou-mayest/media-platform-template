using MassTransit;
using MediatR;
using Profiles.Application.ActorProfiles.Commands.DeleteActorProfileByUserId;
using Users.Contracts.IntegrationEvents;

namespace Profiles.Application.Consumers;


internal sealed class UserDeletedConsumer(ISender sender) : IConsumer<UserDeletedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<UserDeletedIntegrationEvent> context)
    {
        var result = await sender.Send(
            new DeleteActorProfileByUserIdCommand(context.Message.UserId),
            context.CancellationToken);

        if (result.IsFailure)
        {
            throw new InvalidOperationException(
                $"Could not delete the profile for user {context.Message.UserId}: " +
                string.Join("; ", result.Errors.Select(e => $"{e.Code} {e.Message}")));
        }
    }
}
