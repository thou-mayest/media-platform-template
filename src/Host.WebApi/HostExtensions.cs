using MassTransit;
using Microsoft.EntityFrameworkCore;
using Profiles.Infrastructure;
using Profiles.Infrastructure.Persistence;
using Users.Infrastracture.Persistence;
using Users.Infrastracture;
using Users.Presentation;

namespace Host.WebApi;

public static class HostExtensions
{

    public static async Task ApplyMigrations(this WebApplication app)
    {
        await MigrateModuleDbAsync<UsersDbContext>(app);
        await MigrateModuleDbAsync<ProfilesDbContext>(app);
    }

    public static TBuilder RegisterModules<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddUsersInfrastructure(builder.Configuration);
        builder.Services.AddUsersPresentation();

        builder.Services.AddProfilesInfrastructure(builder.Configuration);

        builder.Services.AddMessageBus();

        return builder;
    }

   
    private static IServiceCollection AddMessageBus(this IServiceCollection services)
    {
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


    private static async Task MigrateModuleDbAsync<TDbContext>(this WebApplication app) where TDbContext : DbContext
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TDbContext>();
        await db.Database.MigrateAsync();
    }
}
