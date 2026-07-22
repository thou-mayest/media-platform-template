using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Users.Presentation.Authorization;

namespace Users.Presentation;

public static class Extension
{
    public static IServiceCollection AddUsersPresentation(this IServiceCollection services)
    {
        services.AddControllers()
            .AddApplicationPart(typeof(Extension).Assembly);

        services.AddUsersAuthorization();

        return services;
    }
}

