using Microsoft.Extensions.DependencyInjection;

namespace Storage.Presentation;

public static class Extension
{
    public static IServiceCollection AddStoragePresentation(this IServiceCollection services)
    {
        services.AddControllers()
            .AddApplicationPart(typeof(Extension).Assembly);

        return services;
    }
}
