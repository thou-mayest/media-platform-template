using Users.Common;

namespace Users.Presentation.Users;

public sealed record CreateUserRequest(string Name, string Email, string Password, Role Role);
public sealed record UpdateUserRequest(string Name, string Email, string Password, Role Role);
