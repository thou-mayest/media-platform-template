using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Application.Abstractions;
using Users.Application;
using Users.Infrastracture.Persistence;
using Users.Infrastracture.Health;
using Users.Infrastracture.Security;

namespace Users.Infrastracture;

internal static class DependencyInjection
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
        var connectionString = configuration.GetConnectionString("MainDb")
            ?? throw new InvalidOperationException("Connection string 'MainDb' is not configured.");

        services.AddDbContextPool<UsersDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
        npgsql.MigrationsHistoryTable("__UsersMigrations", "Users")));

        services.AddHealthChecks()
            .AddCheck<MainDbHealthCheck>("MainDb", tags: ["ready"]);

        return services;
    }

    private static IServiceCollection AddUsersApplication(this IServiceCollection services)
    {
        services.InitializeApplication();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHashingService, AspNetPasswordHashingService>();
        return services;
    }
}
