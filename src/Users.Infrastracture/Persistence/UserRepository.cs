using Users.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using SharedKernal.Messaging;
using Users.Domain;

namespace Users.Infrastracture.Persistence;

internal class UserRepository : IUserRepository
{
    private readonly UsersDbContext _context;
    private readonly IDomainEventDispatcher _dispatcher;

    public UserRepository(UsersDbContext context, IDomainEventDispatcher dispatcher)
    {
        _context = context;
        _dispatcher = dispatcher;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FindAsync([id], cancellationToken);
    }

    public void Remove(User user)
    {
        _context.Users.Remove(user);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregates = _context.ChangeTracker
            .Entries<User>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        aggregates.ForEach(a => a.ClearDomainEvents());

        var result = await _context.SaveChangesAsync(cancellationToken);

        await _dispatcher.DispatchAsync(domainEvents, cancellationToken);

        return result;
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
    }
}
