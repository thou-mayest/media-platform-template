using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Application;

public static class ApplicationExtension
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ApplicationExtension).Assembly));

        return services;
    }
}
