using Microsoft.Extensions.DependencyInjection;
using Posts.Presentation.Authorization;

namespace Posts.Presentation;

public static class Extension
{
    public static IServiceCollection AddPostsPresentation(this IServiceCollection services)
    {
        services.AddControllers().AddApplicationPart(typeof(Extension).Assembly);
        services.AddPostsAuthorization();
        return services;
    }
}
