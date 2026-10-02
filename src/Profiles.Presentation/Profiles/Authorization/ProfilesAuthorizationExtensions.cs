using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Entities.Enums;

namespace Profiles.Presentation.Authorization;

internal static class ProfilesAuthorizationExtensions
{
    internal static IServiceCollection AddProfilesAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(ProfilesPolicies.RequireAdmin, policy =>
                policy.RequireRole(nameof(Role.Admin)));

        return services;
    }
}