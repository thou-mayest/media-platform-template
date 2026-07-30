using SharedKernal.Messaging;
using Users.Application.Abstractions;
using Users.Application.Users.Exceptions;

namespace Users.Application.Users.Commands.DeleteUser;

internal sealed class DeleteUserCommandHandler(IUserRepository userRepository)
    : ICommandHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new UserNotFoundException(request.Id);

        userRepository.Remove(user);
        await userRepository.SaveChangesAsync(cancellationToken);
    }
}
