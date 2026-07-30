using System.ComponentModel.DataAnnotations;
using Host.WebApi.ArtworkViews;
using Microsoft.EntityFrameworkCore;
using Users.Common;
using Users.Domain;
using Users.Domain.Abstractions;
using Users.Infrastracture.Persistence;

namespace Host.WebApi;

public static class HostExtensions
{
    public static async Task MigrateUsersDbAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        await db.Database.MigrateAsync();
    }

    public static async Task MigrateArtworkViewsDbAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ArtworkViewsDbContext>();
        await db.Database.MigrateAsync();
    }

    public static async Task SeedBootstrapAdminAsync(this WebApplication app)
    {
        var email = app.Configuration["BootstrapAdmin:Email"];
        var password = app.Configuration["BootstrapAdmin:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;
        if (!new EmailAddressAttribute().IsValid(email) || password.Length is < 8 or > 128)
            throw new InvalidOperationException("Bootstrap administrator configuration is invalid.");

        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        await using var transaction = await db.Database.BeginTransactionAsync();
        await db.Database.ExecuteSqlRawAsync("SELECT pg_advisory_xact_lock(73420519)");

        var normalizedEmail = email.Trim().ToLowerInvariant();
        var existing = await db.Users.SingleOrDefaultAsync(user => user.Email.Value == normalizedEmail);
        if (existing is not null)
        {
            if (existing.Role != Role.Admin)
                throw new InvalidOperationException("Bootstrap administrator email belongs to a non-administrator account.");
            if (existing.Password.HashedValue == Users.Domain.ValueObjects.Password.InvalidatedHash)
            {
                var passwordResult = existing.ChangePassword(password, hasher);
                if (passwordResult.IsFailure)
                    throw new InvalidOperationException(passwordResult.Error.Message);
                existing.RotateVersion();
                await db.SaveChangesAsync();
            }
            await transaction.CommitAsync();
            return;
        }

        var result = User.Create(
            app.Configuration["BootstrapAdmin:Name"] ?? "Administrator",
            normalizedEmail,
            password,
            Role.Admin,
            hasher);
        if (result.IsFailure)
            throw new InvalidOperationException(result.Error.Message);

        db.Users.Add(result.Value);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}
