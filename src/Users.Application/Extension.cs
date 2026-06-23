using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Users.Application;

public static class Extension
{
    public static IServiceCollection AddUsersApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));


        return services;
    }
}
