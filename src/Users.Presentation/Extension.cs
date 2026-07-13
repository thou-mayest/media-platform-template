using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Users.Presentation.Authorization;
using Users.Presentation.Users;

namespace Users.Presentation;

public static class Extension
{
    public static IServiceCollection AddUsersPresentation(this IServiceCollection services)
    {
        services.AddUsersAuthorization();
        return services;
    }

    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapUserEndpoints();
        return app;
    }
}
