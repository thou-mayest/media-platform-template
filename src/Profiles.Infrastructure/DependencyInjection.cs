using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Profiles.Application;
using Profiles.Application.Abstractions;
using Profiles.Application.Messaging;
using Profiles.Infrastructure.Persistence;
using SharedKernal.Messaging;

namespace Profiles.Infrastructure;

internal static class DependencyInjection
{
    public static IServiceCollection AddProfilesInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext(configuration);

        services.AddProfilesApplication();

        return services;
    }

    private static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreConnectionString");

        services.AddDbContextPool<ProfilesDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ProfilesMigrations", "Profiles")));

        return services;
    }

    private static IServiceCollection AddProfilesApplication(this IServiceCollection services)
    {
        services.InitializeApplication();

        services.AddScoped<IActorProfileRepository, ActorProfileRepository>();

        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

        return services;
    }
}
