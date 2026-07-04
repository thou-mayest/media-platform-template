using SharedKernal.Messaging;
using SharedKernal.Results;
using Users.Application.Abstractions;
using Users.Domain;
using Users.Domain.Abstractions;

namespace Users.Application.Users.Commands.CreateUser;

internal sealed class CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    : ICommandHandler<CreateUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var result = User.Create(request.Name, request.Email, request.Password, request.Role, passwordHasher);

        // Propagate domain validation errors instead of throwing —
        // the caller decides how to translate them into an HTTP response.
        if (result.IsFailure)
            return Result.Failure<Guid>(result.Errors);

        var user = result.Value;
        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(user.Id);
    }
}
