using Users.Domain;

namespace Users.Application.Users;

public sealed record UserDto(
    Guid Id,
    string Name,
    string Email,
    Role Role,
    DateTime CreatedDate,
    DateTime? UpdateDate);
