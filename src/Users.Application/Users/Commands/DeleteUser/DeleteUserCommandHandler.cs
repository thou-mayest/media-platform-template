using SharedKernal.Messaging;
using Users.Application.Abstractions;

namespace Users.Application.Users.Commands.DeleteUser;

internal sealed class DeleteUserCommandHandler(IUserRepository userRepository)
    : ICommandHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{request.Id}' was not found.");

        userRepository.Remove(user);
        await userRepository.SaveChangesAsync(cancellationToken);
    }
}
