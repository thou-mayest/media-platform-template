namespace Users.Presentation.Authorization;

/// <summary>
/// Centralized names for authorization policies owned by the Users module.
/// Keeping them here avoids magic strings in endpoint definitions.
/// </summary>
public static class UsersPolicies
{
    public const string RequireAdmin = "Users.RequireAdmin";
    public const string RequirePremiumUser = "Users.RequirePremiumUser";
    public const string CanManageUsers = "Users.CanManageUsers";
}
