using Catalog.Application.Abstractions;
using Catalog.Application.Catalog;
using Catalog.Domain;

namespace CleanModular.UnitTests.Catalog;

public class CatalogCommandTests
{
    private sealed class FakeCatalogRepository : ICatalogRepository
    {
        public List<Category> Categories { get; } = [];
        public List<Tag> Tags { get; } = [];
        public List<Video> Videos { get; } = [];

        public Task<IReadOnlyList<Creator>> GetCreatorsAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<Creator>>([]);

        public Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<Category>>(Categories);

        public Task<IReadOnlyList<Tag>> GetTagsAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<Tag>>(Tags);

        public Task<IReadOnlyList<Video>> GetVideosAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<Video>>(Videos);

        public Task<Creator?> GetCreatorBySlugAsync(string slug, CancellationToken cancellationToken) =>
            Task.FromResult<Creator?>(null);

        public Task<Category?> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken) =>
            Task.FromResult(Categories.FirstOrDefault(c => c.Slug == slug));

        public Task<Tag?> GetTagBySlugAsync(string slug, CancellationToken cancellationToken) =>
            Task.FromResult(Tags.FirstOrDefault(t => t.Slug == slug));

        public Task<Video?> GetVideoBySlugAsync(string slug, CancellationToken cancellationToken) =>
            Task.FromResult(Videos.FirstOrDefault(v => v.Slug == slug));

        public Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(Categories.FirstOrDefault(c => c.Id == id));

        public Task<Category?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken) =>
            Task.FromResult(Categories.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)));

        public Task<Category?> GetDefaultCategoryAsync(CancellationToken cancellationToken) =>
            Task.FromResult(Categories.FirstOrDefault(c => c.Slug == Category.DefaultSlug));

        public Task AddCategoryAsync(Category category, CancellationToken cancellationToken)
        {
            Categories.Add(category);
            return Task.CompletedTask;
        }

        public void UpdateCategory(Category category) { }

        public Task DeleteCategoryAsync(Category category, Category fallbackCategory, CancellationToken cancellationToken)
        {
            foreach (var video in Videos.Where(v => v.CategoryId == category.Id))
            {
                video.ChangeCategory(fallbackCategory.Id);
            }
            Categories.Remove(category);
            return Task.CompletedTask;
        }

        public Task<Tag?> GetTagByIdAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(Tags.FirstOrDefault(t => t.Id == id));

        public Task<Tag?> GetTagByNameAsync(string name, CancellationToken cancellationToken) =>
            Task.FromResult(Tags.FirstOrDefault(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase)));

        public Task<Tag?> GetDefaultTagAsync(CancellationToken cancellationToken) =>
            Task.FromResult(Tags.FirstOrDefault(t => t.Slug == Tag.DefaultSlug));

        public Task AddTagAsync(Tag tag, CancellationToken cancellationToken)
        {
            Tags.Add(tag);
            return Task.CompletedTask;
        }

        public void UpdateTag(Tag tag) { }

        public Task DeleteTagAsync(Tag tag, Tag fallbackTag, CancellationToken cancellationToken)
        {
            foreach (var video in Videos)
            {
                video.RemoveTag(tag.Id);
                if (video.VideoTags.Count == 0)
                {
                    video.AddTag(fallbackTag.Id);
                }
            }
            Tags.Remove(tag);
            return Task.CompletedTask;
        }

        public Task<Video?> GetVideoByIdAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(Videos.FirstOrDefault(v => v.Id == id));

        public void UpdateVideo(Video video) { }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
            Task.FromResult(1);
    }

    [Fact]
    public async Task DeleteCategory_Should_Throw_When_Deleting_Default_Category()
    {
        var repo = new FakeCatalogRepository();
        var defaultCategory = new Category(Category.DefaultName, Category.DefaultSlug);
        await repo.AddCategoryAsync(defaultCategory, CancellationToken.None);

        var handler = new DeleteCategoryCommandHandler(repo);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new DeleteCategoryCommand(defaultCategory.Id), CancellationToken.None));
    }

    [Fact]
    public async Task DeleteCategory_Should_Reassign_Videos_To_DefaultCategory()
    {
        var repo = new FakeCatalogRepository();
        var defaultCategory = new Category(Category.DefaultName, Category.DefaultSlug);
        var oldCategory = new Category("Old Category", "old-category");
        await repo.AddCategoryAsync(defaultCategory, CancellationToken.None);
        await repo.AddCategoryAsync(oldCategory, CancellationToken.None);

        var video = new Video(
            Guid.NewGuid(),
            oldCategory.Id,
            "Sample Video",
            "sample-video",
            2024,
            "Digital",
            "1920x1080",
            1.77,
            "#FFFFFF",
            1);
        repo.Videos.Add(video);

        var handler = new DeleteCategoryCommandHandler(repo);
        var result = await handler.Handle(new DeleteCategoryCommand(oldCategory.Id), CancellationToken.None);

        Assert.True(result);
        Assert.DoesNotContain(oldCategory, repo.Categories);
        Assert.Equal(defaultCategory.Id, video.CategoryId);
    }

    [Fact]
    public async Task CreateCategory_Should_Create_And_Slugify()
    {
        var repo = new FakeCatalogRepository();
        var handler = new CreateCategoryCommandHandler(repo);

        var result = await handler.Handle(new CreateCategoryCommand("Fine Art & Design"), CancellationToken.None);

        Assert.Equal("Fine Art & Design", result.Name);
        Assert.Equal("fine-art-design", result.Slug);
        Assert.Single(repo.Categories);
    }

    [Fact]
    public async Task DeleteTag_Should_Throw_When_Deleting_Default_Tag()
    {
        var repo = new FakeCatalogRepository();
        var defaultTag = new Tag(Tag.DefaultName, Tag.DefaultSlug);
        await repo.AddTagAsync(defaultTag, CancellationToken.None);

        var handler = new DeleteTagCommandHandler(repo);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new DeleteTagCommand(defaultTag.Id), CancellationToken.None));
    }

    [Fact]
    public async Task DeleteTag_Should_Reassign_Videos_To_DefaultTag_When_No_Other_Tags()
    {
        var repo = new FakeCatalogRepository();
        var defaultTag = new Tag(Tag.DefaultName, Tag.DefaultSlug);
        var oldTag = new Tag("Old Tag", "old-tag");
        await repo.AddTagAsync(defaultTag, CancellationToken.None);
        await repo.AddTagAsync(oldTag, CancellationToken.None);

        var video = new Video(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Sample Video",
            "sample-video",
            2024,
            "Digital",
            "1920x1080",
            1.77,
            "#FFFFFF",
            1);
        video.AddTag(oldTag.Id);
        repo.Videos.Add(video);

        var handler = new DeleteTagCommandHandler(repo);
        var result = await handler.Handle(new DeleteTagCommand(oldTag.Id), CancellationToken.None);

        Assert.True(result);
        Assert.DoesNotContain(oldTag, repo.Tags);
        Assert.Single(video.VideoTags);
        Assert.Equal(defaultTag.Id, video.VideoTags.First().TagId);
    }
}

