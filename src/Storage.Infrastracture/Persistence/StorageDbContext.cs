using Microsoft.EntityFrameworkCore;
using SharedKernal.Messaging.Outbox;
using Storage.Domain;

namespace Storage.Infrastracture.Persistence;

internal class StorageDbContext : DbContext
{
    public StorageDbContext(DbContextOptions<StorageDbContext> options) : base(options)
    {
    }

    public DbSet<MediaAsset> MediaAssets { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Storage");

        modelBuilder.Entity<MediaAsset>(entity =>
        {
            entity.HasKey(m => m.Id);

            entity.Property(m => m.FileName)
                .IsRequired()
                .HasMaxLength(512);

            entity.Property(m => m.OriginalFileName)
                .IsRequired()
                .HasMaxLength(512);

            entity.Property(m => m.ContentType)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(m => m.FileSize)
                .IsRequired();

            entity.Property(m => m.StorageProvider)
                .IsRequired()
                .HasMaxLength(128);

            entity.Property(m => m.BucketName)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(m => m.StorageKey)
                .IsRequired()
                .HasMaxLength(1024);

            entity.Property(m => m.Url)
                .IsRequired()
                .HasMaxLength(2048);

            entity.HasIndex(m => m.StorageKey);
            entity.HasIndex(m => m.CreatedDate);
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

            builder.HasIndex(x => x.ProcessedOnUtc);
        });

        base.OnModelCreating(modelBuilder);
    }
}
