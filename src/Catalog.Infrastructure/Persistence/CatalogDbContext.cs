using Catalog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Creator> Creators => Set<Creator>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<Video> Videos => Set<Video>();

    public DbSet<VideoTag> VideoTags => Set<VideoTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Creator>(entity =>
        {
            entity.ToTable("catalog_creators");
            entity.HasKey(creator => creator.Id);
            entity.HasIndex(creator => creator.Slug).IsUnique();

            entity.Property(creator => creator.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(creator => creator.Slug)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(creator => creator.Location)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(creator => creator.Disciplines)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(creator => creator.Bio)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(creator => creator.Accent)
                .IsRequired()
                .HasMaxLength(32);

            entity.Property(creator => creator.Initials)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(creator => creator.Since)
                .IsRequired()
                .HasMaxLength(10);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("catalog_categories");
            entity.HasKey(category => category.Id);
            entity.HasIndex(category => category.Name).IsUnique();
            entity.HasIndex(category => category.Slug).IsUnique();

            entity.Property(category => category.Name)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(category => category.Slug)
                .IsRequired()
                .HasMaxLength(120);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("catalog_tags");
            entity.HasKey(tag => tag.Id);
            entity.HasIndex(tag => tag.Name).IsUnique();
            entity.HasIndex(tag => tag.Slug).IsUnique();

            entity.Property(tag => tag.Name)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(tag => tag.Slug)
                .IsRequired()
                .HasMaxLength(120);
        });

        modelBuilder.Entity<Video>(entity =>
        {
            entity.ToTable("catalog_videos");
            entity.HasKey(video => video.Id);
            entity.HasIndex(video => video.Slug).IsUnique();
            entity.HasIndex(video => video.DisplayOrder).IsUnique();
            entity.HasIndex(video => new { video.Year, video.DisplayOrder });

            entity.Property(video => video.Title)
                .IsRequired()
                .HasMaxLength(240);

            entity.Property(video => video.Slug)
                .IsRequired()
                .HasMaxLength(240);

            entity.Property(video => video.Medium)
                .IsRequired()
                .HasMaxLength(240);

            entity.Property(video => video.Size)
                .IsRequired()
                .HasMaxLength(80);

            entity.Property(video => video.Background)
                .IsRequired()
                .HasMaxLength(500);

            entity.HasOne(video => video.Creator)
                .WithMany(creator => creator.Videos)
                .HasForeignKey(video => video.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(video => video.Category)
                .WithMany(category => category.Videos)
                .HasForeignKey(video => video.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<VideoTag>(entity =>
        {
            entity.ToTable("catalog_video_tags");
            entity.HasKey(link => new { link.VideoId, link.TagId });

            entity.HasOne(link => link.Video)
                .WithMany(video => video.VideoTags)
                .HasForeignKey(link => link.VideoId);

            entity.HasOne(link => link.Tag)
                .WithMany(tag => tag.VideoTags)
                .HasForeignKey(link => link.TagId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
