using Microsoft.Extensions.DependencyInjection;
using Users.Presentation.Authorization;
using System.Text.Json.Serialization;

namespace Users.Presentation;

public static class Extension
{
    public static IServiceCollection AddUsersPresentation(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

            options.JsonSerializerOptions.DefaultIgnoreCondition =
                JsonIgnoreCondition.WhenWritingNull;
        }); ;

        services.AddUsersAuthorization();

        return services;
    }
}

