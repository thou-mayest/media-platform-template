using Microsoft.EntityFrameworkCore;
using Profiles.Domain;
using Profiles.Domain.ValueObjects;

namespace Profiles.Infrastructure.Persistence;

internal class ProfilesDbContext : DbContext
{
    public ProfilesDbContext(DbContextOptions<ProfilesDbContext> options) : base(options)
    {
    }

    public DbSet<ActorProfile> ActorProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Profiles");

        modelBuilder.Entity<ActorProfile>(entity =>
        {
            entity.HasKey(p => p.Id);

            
            entity.OwnsOne(p => p.Slug, slug =>
            {
                slug.Property(s => s.Value)
                    .HasColumnName("Slug")
                    .IsRequired()
                    .HasMaxLength(ProfileSlug.MaxLength);

                slug.HasIndex(s => s.Value).IsUnique();
            });
            entity.HasIndex(p => p.UserId).IsUnique();

            entity.Property(p => p.DisplayName)
                .IsRequired()
                .HasMaxLength(ActorProfile.MaxDisplayNameLength);

            entity.Property(p => p.Profession)
                .IsRequired()
                .HasMaxLength(ActorProfile.MaxProfessionLength);

            entity.Property(p => p.Bio)
                .IsRequired()
                .HasMaxLength(ActorProfile.MaxBioLength);

            entity.Property(p => p.AvatarStorageKey)
                .HasMaxLength(ActorProfile.MaxAvatarKeyLength);

            entity.OwnsMany(p => p.SocialLinks, link =>
            {
                link.ToJson();

                link.Property(l => l.Platform)
                    .HasConversion<string>();

                link.Property(l => l.Url)
                    .IsRequired()
                    .HasMaxLength(SocialLink.MaxUrlLength);
            });

            entity.Navigation(p => p.SocialLinks)
                .HasField("_socialLinks")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            entity.HasIndex(p => new { p.CreatedDate, p.Id })
                .HasFilter("\"IsIndexable\"")
                .HasDatabaseName("IX_ActorProfiles_Indexable_CreatedDate");

            entity.Property(p => p.IsPublished).IsRequired();
            entity.Property(p => p.IsIndexable).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}