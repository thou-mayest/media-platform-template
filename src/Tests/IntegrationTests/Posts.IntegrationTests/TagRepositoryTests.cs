using Microsoft.EntityFrameworkCore;
using Posts.Domain;
using Posts.Infrastructure.Persistence;

namespace Posts.IntegrationTests;

[Collection(PostsDatabaseCollection.Name)]
public sealed class TagRepositoryTests(PostsDatabaseFixture fixture) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await using var context = fixture.CreateContext();
        await context.Database.ExecuteSqlRawAsync(
            "TRUNCATE TABLE \"Posts\".\"Posts\", \"Posts\".\"Tags\" CASCADE");
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetOrCreate_ReusesExistingTagsByNormalizedName()
    {
        await using (var context = fixture.CreateContext())
        {
            await new TagRepository(context).GetOrCreateAsync([Tag.Create("DotNet").Value], CancellationToken.None);
        }

        await using (var context = fixture.CreateContext())
        {
            var tags = await new TagRepository(context).GetOrCreateAsync(
                [Tag.Create("#dotnet").Value, Tag.Create("DOTNET").Value, Tag.Create("Astro").Value],
                CancellationToken.None);

            Assert.Equal(2, tags.Count);
            Assert.Contains(tags, tag => tag.Name == "DotNet");
            Assert.Contains(tags, tag => tag.Name == "Astro");
            Assert.Equal(2, await context.Tags.CountAsync());
        }
    }

    [Fact]
    public async Task GetOrCreate_ConcurrentCallsForSameNewTag_ProduceOneRow()
    {
        var calls = Enumerable.Range(0, 8).Select(async _ =>
        {
            await using var context = fixture.CreateContext();
            var tags = await new TagRepository(context).GetOrCreateAsync(
                [Tag.Create("racing").Value], CancellationToken.None);
            return Assert.Single(tags).Id;
        });

        var ids = await Task.WhenAll(calls);

        Assert.Single(ids.Distinct());
    }

    [Fact]
    public async Task PostWithResolvedTags_PersistsJoinRows()
    {
        await using var context = fixture.CreateContext();
        var tags = await new TagRepository(context).GetOrCreateAsync(
            [Tag.Create("one").Value, Tag.Create("two").Value], CancellationToken.None);
        var post = Post.Create(Guid.NewGuid(), Guid.NewGuid(), "Title", "Description", null,
            "https://media.example/post.jpg", "Alt").Value;
        foreach (var tag in tags) post.AddTag(tag);

        context.Add(post);
        await context.SaveChangesAsync();

        await using var verify = fixture.CreateContext();
        var names = await verify.Posts.Where(p => p.Id == post.Id)
            .SelectMany(p => p.Tags).Select(pt => pt.Tag!.Name).OrderBy(n => n).ToListAsync();
        Assert.Equal(["one", "two"], names);
    }
}
