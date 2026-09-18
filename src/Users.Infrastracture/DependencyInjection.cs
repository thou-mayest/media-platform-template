using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernal.Messaging;
using Users.Application;
using Users.Application.Abstractions;
using Users.Application.Messaging;
using Users.Domain.Abstractions;
using Users.Infrastructure.Interceptors;
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

        services.AddUsersApplication(configuration);

        return services;
    }

    private static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreConnectionString");

        // Register the interceptor as Scoped to align with the HTTP request and DbContext lifetime
        services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();

        // Configure DbContextPool and register the interceptor
        services.AddDbContextPool<UsersDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<ConvertDomainEventsToOutboxMessagesInterceptor>();

            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__UsersMigrations", "Users"))
                   .AddInterceptors(interceptor);
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

        return services;
    }
}