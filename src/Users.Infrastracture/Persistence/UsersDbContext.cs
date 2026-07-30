using Microsoft.EntityFrameworkCore;
using Users.Domain;

namespace Users.Infrastracture.Persistence;

internal class UsersDbContext : DbContext
{
    internal const string EmailUniqueIndexName = "UX_Users_Email";

    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Users");

        // move to seperate configuration files later
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable(table => table.HasCheckConstraint(
                "CK_Users_Role",
                "\"Role\" IN (1, 2, 3)"));

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName(EmailUniqueIndexName);

            entity.Property(u => u.PasswordHash)
                .IsRequired();

            entity.Property(u => u.Role)
                .IsRequired();

            entity.Property(u => u.Version)
                .IsConcurrencyToken();
        });

        base.OnModelCreating(modelBuilder);
    }
}
