using Users.Common;

namespace Users.Application.Users;

internal sealed record UserDto(
    Guid Id,
    string Name,
    string Email,
    Role Role,
    DateTime CreatedDate,
    DateTime? UpdateDate);
