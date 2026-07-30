using Users.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Users.Domain;
using Npgsql;
using Users.Application.Users.Exceptions;

namespace Users.Infrastracture.Persistence;

internal class UserRepository : IUserRepository
{
    private readonly UsersDbContext _context;

    public UserRepository(UsersDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .OrderBy(user => user.Name)
            .ThenBy(user => user.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FindAsync([id], cancellationToken);
    }

    public Task<User?> GetByEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default) =>
        _context.Users.SingleOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);

    public Task<bool> EmailExistsAsync(
        string normalizedEmail,
        Guid? excludingUserId = null,
        CancellationToken cancellationToken = default) =>
        _context.Users.AnyAsync(
            user => user.Email == normalizedEmail &&
                    (!excludingUserId.HasValue || user.Id != excludingUserId.Value),
            cancellationToken);

    public void Remove(User user)
    {
        _context.Users.Remove(user);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation,
                ConstraintName: UsersDbContext.EmailUniqueIndexName
            })
        {
            throw new EmailAlreadyExistsException(exception);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new UserConcurrencyException(exception);
        }
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
    }
}
