using Microsoft.Extensions.DependencyInjection;
using Profiles.Presentation.Authorization;

namespace Profiles.Presentation;

public static class Extension
{
   
    public static IServiceCollection AddProfilesPresentation(this IServiceCollection services)
    {
        services.AddProfilesAuthorization();

        return services;
    }
}