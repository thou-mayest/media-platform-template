using Microsoft.EntityFrameworkCore;
using Users.Domain;

namespace Users.Infrastracture.Persistence;

public class UsersDbContext : DbContext
{
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
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(u => u.Password)
                .IsRequired();

            entity.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(100);
        });

        base.OnModelCreating(modelBuilder);
    }
}
