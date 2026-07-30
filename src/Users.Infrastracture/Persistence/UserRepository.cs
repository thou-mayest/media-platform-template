using Users.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using SharedKernal.Messaging;
using Users.Domain;
using Npgsql;

namespace Users.Infrastracture.Persistence;

internal class UserRepository(UsersDbContext context, IDomainEventDispatcher dispatcher) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Users.ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {

        return await context.Users.FindAsync([id], cancellationToken);
    }

    public Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default) =>
        context.Users.SingleOrDefaultAsync(user => user.Email.Value == normalizedEmail, cancellationToken);

    public Task<bool> EmailExistsAsync(
        string normalizedEmail,
        Guid? excludingUserId = null,
        CancellationToken cancellationToken = default) =>
        context.Users.AnyAsync(
            user => user.Email.Value == normalizedEmail &&
                    (!excludingUserId.HasValue || user.Id != excludingUserId.Value),
            cancellationToken);

    public void Remove(User user)
    {
        context.Users.Remove(user);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregates = context.ChangeTracker
            .Entries<User>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        aggregates.ForEach(a => a.ClearDomainEvents());

        int result;
        try
        {
            result = await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation,
                ConstraintName: "UX_Users_Email"
            })
        {
            throw new UserEmailConflictException(exception);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new UserConcurrencyException(exception);
        }

        await dispatcher.DispatchAsync(domainEvents, cancellationToken);

        return result;
    }

    public void Update(User user)
    {
        context.Users.Update(user);
    }
}
