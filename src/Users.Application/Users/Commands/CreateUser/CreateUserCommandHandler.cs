using SharedKernal.Messaging;
using Users.Application.Abstractions;
using Users.Domain;
using Users.Common;

namespace Users.Application.Users.Commands.CreateUser;

internal sealed class CreateUserCommandHandler(IUserRepository userRepository)
    : ICommandHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User(request.Name, request.Email, request.Password, request.Role);

        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
