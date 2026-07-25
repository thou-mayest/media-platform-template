namespace Users.Presentation.Auth;

public sealed record LoginRequest(string Email, string Password);

public sealed record SignupRequest(string Name, string Email, string Password);

public sealed record LoginResponse(string Token, Guid UserId, string Name, string Email);
