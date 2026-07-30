using SharedKernal.Messaging;
using SharedKernal.Results;
using Users.Application.Abstractions;
using Users.Domain.Abstractions;
namespace Users.Application.Users.Commands.UpdateUser;

internal sealed class UpdateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    : ICommandHandler<UpdateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
            return Error.NotFound(ErrorCodes.NotFound, $"user with Id {request.Id} not found");

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await userRepository.EmailExistsAsync(normalizedEmail, request.Id, cancellationToken))
            return Error.Conflict("User.EmailExists", "A user with that email already exists.");

        var profileResult = user.UpdateProfile(request.Name, request.Email);
        if (profileResult.IsFailure)
            return profileResult.Error;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var passwordResult = user.ChangePassword(request.Password, passwordHasher);
            if (passwordResult.IsFailure)
                return passwordResult.Error;
        }

        if (request.Role.HasValue)
        {
            user.ChangeRole(request.Role.Value);
        }
        user.RotateVersion();

        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);
        return user.ToDto();
    }
}
