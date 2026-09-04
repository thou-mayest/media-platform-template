using Microsoft.EntityFrameworkCore;
using Users.Domain;
using SharedKernel.Messaging;
namespace Users.Infrastracture.Persistence;

internal class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Users");

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