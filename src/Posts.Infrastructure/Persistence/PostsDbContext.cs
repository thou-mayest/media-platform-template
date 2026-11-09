using Microsoft.EntityFrameworkCore;
using Posts.Domain;

namespace Posts.Infrastructure.Persistence;

internal sealed class PostsDbContext(DbContextOptions<PostsDbContext> options) : DbContext(options)
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Posts");

        modelBuilder.Entity<Post>(post =>
        {
            post.HasKey(value => value.Id);
            post.Property(value => value.AuthorId).IsRequired();
            post.Property(value => value.MediaAssetId).IsRequired();
            post.Property(value => value.Title).HasMaxLength(200).IsRequired();
            post.Property(value => value.Description).HasMaxLength(2_000).IsRequired();
            post.Property(value => value.CategoryId).IsRequired();
            post.Property(value => value.MediaUrl).HasMaxLength(2_048).IsRequired();
            post.Property(value => value.AltText).HasMaxLength(500).IsRequired();
            post.Property(value => value.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
            post.Property(value => value.ViewCount).HasDefaultValue(0L).IsRequired();
            post.HasIndex(value => new { value.Status, value.PublishedAt });
            post.HasIndex(value => new { value.Status, value.ViewCount });
            post.HasIndex(value => new { value.Status, value.CategoryId });
            post.HasIndex(value => value.AuthorId);
            post.HasIndex(value => value.MediaAssetId);

            post.HasOne(value => value.Category)
                .WithMany(value => value.Posts)
                .HasForeignKey(value => value.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            post.OwnsMany(value => value.Tags, tag =>
            {
                tag.ToTable("PostTags", "Posts");
                tag.WithOwner().HasForeignKey("PostId");
                tag.HasKey("PostId", "NormalizedName");
                tag.Property(value => value.Name).HasMaxLength(50).IsRequired();
                tag.Property(value => value.NormalizedName).HasMaxLength(50).IsRequired();
            });

            post.Navigation(value => value.Tags).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<Category>(category =>
        {
            category.HasKey(value => value.Id);
            category.Property(value => value.Name).HasMaxLength(100).IsRequired();
            category.Property(value => value.NormalizedName).HasMaxLength(100).IsRequired();
            category.HasIndex(value => value.NormalizedName).IsUnique();
            category.HasData(new
            {
                Id = Category.DefaultId,
                Name = Category.DefaultName,
                NormalizedName = Category.Normalize(Category.DefaultName),
                CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Version = Guid.Parse("00000000-0000-0000-0000-000000000002")
            });
        });

        base.OnModelCreating(modelBuilder);
    }
}
