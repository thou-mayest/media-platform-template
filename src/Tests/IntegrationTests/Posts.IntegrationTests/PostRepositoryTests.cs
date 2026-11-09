using Microsoft.EntityFrameworkCore;
using Posts.Application.Posts;
using Posts.Domain;
using Posts.Infrastructure.Persistence;

namespace Posts.IntegrationTests;

[Collection(PostsDatabaseCollection.Name)]
public sealed class PostRepositoryTests(PostsDatabaseFixture fixture) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await using var context = fixture.CreateContext();
        await context.Database.ExecuteSqlRawAsync(
            "TRUNCATE TABLE \"Posts\".\"Posts\" CASCADE; DELETE FROM \"Posts\".\"Categories\" WHERE \"Id\" <> '00000000-0000-0000-0000-000000000001'");
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Discovery_ReturnsOnlyPublishedPostsAndSupportsFilters()
    {
        await using var context = fixture.CreateContext();
        var painting = Category.Create("Painting").Value;
        var published = CreatePost("Market at Noon", painting, PostTag.Create("Abstract").Value, true);
        var draft = CreatePost("Hidden draft", painting, PostTag.Create("Abstract").Value, false);
        context.AddRange(painting, published, draft);
        await context.SaveChangesAsync();
        var repository = new PostRepository(context);

        var result = await repository.GetPublishedAsync(
            "market", "painting", "#ABSTRACT", PostSort.Newest, 1, 20, CancellationToken.None);

        var item = Assert.Single(result.Items);
        Assert.Equal(published.Id, item.Id);
        Assert.Equal(["Abstract"], item.Tags);
    }

    [Fact]
    public async Task Facets_ExcludeDraftOnlyValues()
    {
        await using var context = fixture.CreateContext();
        var visibleCategory = Category.Create("Painting").Value;
        var hiddenCategory = Category.Create("Private").Value;
        context.AddRange(visibleCategory, hiddenCategory);
        context.Add(CreatePost("Visible", visibleCategory, PostTag.Create("Abstract").Value, true));
        context.Add(CreatePost("Hidden", hiddenCategory, PostTag.Create("Hidden").Value, false));
        await context.SaveChangesAsync();
        var repository = new PostRepository(context);

        var facets = await repository.GetPublishedFacetsAsync(CancellationToken.None);

        Assert.Equal("Painting", Assert.Single(facets.Categories).Name);
        Assert.Equal("Abstract", Assert.Single(facets.Tags).Name);
    }

    [Fact]
    public async Task IncrementViewCount_IsAtomicAndRejectsDrafts()
    {
        await using var context = fixture.CreateContext();
        var paintingCategory = Category.Create("Painting").Value;
        var digitalCategory = Category.Create("Digital").Value;
        var published = CreatePost("Visible", paintingCategory, PostTag.Create("One").Value, true);
        var draft = CreatePost("Hidden", digitalCategory, PostTag.Create("Two").Value, false);
        context.AddRange(paintingCategory, digitalCategory, published, draft);
        await context.SaveChangesAsync();
        var repository = new PostRepository(context);

        Assert.True(await repository.IncrementPublishedViewCountAsync(published.Id, CancellationToken.None));
        Assert.False(await repository.IncrementPublishedViewCountAsync(draft.Id, CancellationToken.None));
        context.ChangeTracker.Clear();
        Assert.Equal(1, (await context.Posts.SingleAsync(post => post.Id == published.Id)).ViewCount);
    }

    [Fact]
    public async Task PopularSortAndPagination_ReturnStableResults()
    {
        await using var context = fixture.CreateContext();
        var painting = Category.Create("Painting").Value;
        var oldest = CreatePost("Oldest", painting, PostTag.Create("oldest").Value, true,
            new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var popular = CreatePost("Popular", painting, PostTag.Create("popular").Value, true,
            new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc));
        var newest = CreatePost("Newest", painting, PostTag.Create("newest").Value, true,
            new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc));
        context.AddRange(painting, oldest, popular, newest);
        await context.SaveChangesAsync();
        var repository = new PostRepository(context);
        await repository.IncrementPublishedViewCountAsync(popular.Id, CancellationToken.None);
        await repository.IncrementPublishedViewCountAsync(popular.Id, CancellationToken.None);

        var firstPage = await repository.GetPublishedAsync(
            null, null, null, PostSort.Popular, 1, 2, CancellationToken.None);
        var secondPage = await repository.GetPublishedAsync(
            null, null, null, PostSort.Popular, 2, 2, CancellationToken.None);

        Assert.Equal(3, firstPage.TotalCount);
        Assert.Equal(2, firstPage.TotalPages);
        Assert.Equal(popular.Id, firstPage.Items[0].Id);
        Assert.Single(secondPage.Items);
    }

    [Fact]
    public async Task GetById_NeverReturnsDrafts()
    {
        await using var context = fixture.CreateContext();
        var painting = Category.Create("Painting").Value;
        var published = CreatePost("Visible", painting, PostTag.Create("visible").Value, true);
        var draft = CreatePost("Hidden", painting, PostTag.Create("hidden").Value, false);
        context.AddRange(painting, published, draft);
        await context.SaveChangesAsync();
        var repository = new PostRepository(context);

        Assert.NotNull(await repository.GetPublishedByIdAsync(published.Id, CancellationToken.None));
        Assert.Null(await repository.GetPublishedByIdAsync(draft.Id, CancellationToken.None));
    }

    private static Post CreatePost(
        string title,
        Category category,
        PostTag tag,
        bool publish,
        DateTime? publishedAt = null)
    {
        var post = Post.Create(
            Guid.NewGuid(), Guid.NewGuid(), title, $"Description for {title}", category.Id,
            "https://media.example/post.jpg", $"Alt text for {title}").Value;
        post.AddTag(tag);
        if (publish) post.Publish(publishedAt ?? DateTime.UtcNow);
        return post;
    }
}
