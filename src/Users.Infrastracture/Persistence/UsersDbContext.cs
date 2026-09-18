using Microsoft.EntityFrameworkCore;
using SharedKernel.Messaging;
using Users.Domain;

namespace Users.Infrastracture.Persistence;

internal class UsersDbContext : DbContext
{
    // Standard constructor required for DbContext pooling
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Set default schema for the Users module
        modelBuilder.HasDefaultSchema("Users");

        // Entity configuration for User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.OwnsOne(u => u.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(256);
            });

            entity.OwnsOne(u => u.Password, password =>
            {
                password.Property(p => p.HashedValue)
                    .HasColumnName("Password")
                    .IsRequired();
            });

            entity.Property(u => u.Role)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(100);
        });

        // Entity configuration for OutboxMessage
        modelBuilder.Entity<OutboxMessage>(builder =>
        {
            builder.ToTable("OutboxMessages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Content)
                .IsRequired();

            builder.Property(x => x.OccurredOnUtc)
                .IsRequired();

            builder.Property(x => x.Error)
                .HasMaxLength(2000);
        });

        base.OnModelCreating(modelBuilder);
    }
}