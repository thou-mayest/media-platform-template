using SharedKernal.Messaging;
using Users.Application.Abstractions;

namespace Users.Application.Users.Queries.GetAllUsers;

internal sealed class GetAllUsersQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetAllUsersQuery, IReadOnlyList<UserDto>>
{
    public async Task<IReadOnlyList<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);

        return users
            .Select(u => new UserDto(u.Id, u.Name, u.Email, u.Role, u.CreatedDate, u.UpdateDate)) // add mappipng later
            .ToList();
    }
}