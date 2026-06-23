using SharedKernal.Messaging;
using Users.Application.Abstractions;

namespace Users.Application.Users.Commands.UpdateUser;

internal sealed class UpdateUserCommandHandler(IUserRepository userRepository)
    : ICommandHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{request.Id}' was not found.");

        user.Update(request.Name, request.Email, request.Password, request.Role);

        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);
    }
}
