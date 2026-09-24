namespace Users.Application.Users;

internal static class UserCacheKeys
{
    public const string Tag = "users";
    public const string AllUsers = "users:all";

    public static string UserById(Guid id) => $"users:{id}";
}
