using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernal.Messaging;
using Users.Application;
using Users.Application.Abstractions;
using Users.Application.Messaging;
using Users.Domain.Abstractions;
using Users.Infrastracture.Persistence;
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

        services.AddUsersMessageBus();

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
            .AddCheck<Health.MainDbHealthCheck>("MainDb", tags: ["ready"]);

        return services;
    }

    private static IServiceCollection AddUsersApplication(this IServiceCollection services)
    {
        services.InitializeApplication();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

        return services;
    }

    private static IServiceCollection AddUsersMessageBus(this IServiceCollection services)
    {

        // later when we have consumer move this into host project and only keep consumer registration here
        services.AddMassTransit(bus =>
        {
            bus.SetKebabCaseEndpointNameFormatter();

            bus.UsingInMemory((ctx, cfg) =>
            {
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
