using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Application;
using Users.Application.Abstractions;
using Users.Application.Messaging;
using Users.Domain.Abstractions;
using Users.Infrastracture.Persistence;
using Users.Infrastracture.Seeding;
using Users.Infrastracture.Security;
using SharedKernal.Messaging.DomainEvents;
using SharedKernal.Messaging.Outbox;

namespace Users.Infrastracture;

internal static class DependencyInjection
{
    public static IServiceCollection AddUsersInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        bool enableSeeding = false)
    {
        services.AddDbContext(configuration, enableSeeding);

        services.AddUsersApplication(configuration);

        return services;
    }

    private static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration configuration,
        bool enableSeeding)
    {
        var connectionString = configuration.GetConnectionString("PostgreConnectionString");
        var seedOptions = configuration
            .GetSection(UserSeedOptions.SectionName)
            .Get<UserSeedOptions>() ?? new UserSeedOptions();
        var passwordHasher = new PasswordHasher();

        // Configure DbContextPool and register the interceptor
        services.AddDbContext<UsersDbContext>((sp, options) =>
        {

            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__UsersMigrations", "Users"))
                   .AddInterceptors(sp.GetRequiredService<DomainEventsInterceptor>());
            if (enableSeeding)
                UserSeeder.Configure(options, seedOptions, passwordHasher);
        });

        return services;
    }

    private static IServiceCollection AddUsersApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.InitializeApplication();

        // Configure JWT Options and Security services
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped<ITokenService, TokenService>();

        // Register repositories and application services
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // Register domain event dispatcher
        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

        // register interceptor
        services.AddTransient<DomainEventsInterceptor>();

        return services;
    }
}
