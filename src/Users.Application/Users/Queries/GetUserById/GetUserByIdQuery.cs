using SharedKernal.Messaging;

namespace Users.Application.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(Guid Id) : IQuery<UserDto?>;
