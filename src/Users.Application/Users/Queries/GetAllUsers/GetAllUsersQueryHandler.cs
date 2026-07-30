using SharedKernal.Messaging;
using SharedKernal.Results;
using Users.Application.Abstractions;

namespace Users.Application.Users.Queries.GetAllUsers;

internal sealed class GetAllUsersQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetAllUsersQuery, Result<IReadOnlyList<UserDto>>>
{
    public async Task<Result<IReadOnlyList<UserDto>>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        if (users.Count == 0)
        {
            return Error.NotFound(ErrorCodes.NotFound, "user list is empty");
        }

        return users.Select(user => user.ToDto()).ToList();
    }
}
