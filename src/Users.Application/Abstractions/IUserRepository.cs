using Users.Domain;

namespace Users.Application.Abstractions;

internal interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(
        string normalizedEmail,
        Guid? excludingUserId = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> GetAllAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);

    void Update(User user);

    void Remove(User user);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
