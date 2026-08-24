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

        var profileResult = user.UpdateProfile(request.Name, request.Email);
        if (profileResult.IsFailure)
            return profileResult.Error;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var passwordResult = user.ChangePassword(request.Password, passwordHasher);
            if (passwordResult.IsFailure)
                return passwordResult.Error;
        }

        user.ChangeRole(request.Role);

        userRepository.Update(user);
        try
        {
            await userRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DuplicateUserEmailException)
        {
            return Error.Conflict("User.EmailExists", "A user with that email already exists.");
        }

        return user.ToDto();
    }
}
