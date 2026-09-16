using Microsoft.AspNetCore.Authorization;

namespace Users.Presentation.Authorization.Requirements;

public sealed class SameUserOrAdminRequirement : IAuthorizationRequirement;
