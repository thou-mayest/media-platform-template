using Microsoft.AspNetCore.Routing;
using Users.Presentation.Users;

namespace Users.Presentation;

public static class Extension
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAuthEndpoints();
        app.MapUserEndpoints();
        return app;
    }
}
