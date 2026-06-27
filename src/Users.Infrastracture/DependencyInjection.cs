using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Application.Abstractions;
using Users.Application;
using Users.Infrastracture.Persistence;

namespace Users.Infrastracture;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext(configuration);

        services.AddUsersApplication();

        return services;
    }

    private static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MainDbConnectionString");
        services.AddDbContext<UsersDbContext>(options =>
            options.UseNpgsql(connectionString));
        return services;
    }

    private static IServiceCollection AddUsersApplication(this IServiceCollection services)
    {
        services.InitializeApplication();

        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}
