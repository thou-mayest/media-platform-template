using Microsoft.Extensions.DependencyInjection;

namespace Profiles.Application;

public static class ApplicationExtension
{
    public static IServiceCollection InitializeApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ApplicationExtension).Assembly));

        return services;
    }
}