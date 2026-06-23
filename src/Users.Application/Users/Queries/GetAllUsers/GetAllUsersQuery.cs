using SharedKernal.Messaging;

namespace Users.Application.Users.Queries.GetAllUsers;

public sealed record GetAllUsersQuery : IQuery<IReadOnlyList<UserDto>>;
