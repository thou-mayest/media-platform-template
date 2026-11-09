using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Entities.Enums;

namespace Posts.Presentation.Authorization;

internal static class PostsAuthorizationExtensions
{
    public static IServiceCollection AddPostsAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(PostsPolicies.RequireAdmin, policy =>
                policy.RequireRole(nameof(Role.Admin)));

        return services;
    }
}
