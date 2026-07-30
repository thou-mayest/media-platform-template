using System.Net;
using System.Net.Http.Json;
using Host.WebApi.ArtworkViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace CleanModular.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public sealed class ArtworkViewTests(ApiFactory factory)
{
    [Fact]
    public async Task UnknownArtwork_IsRejectedWithoutCreatingCounter()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/artwork-views", new
        {
            slug = "unknown-artwork-999"
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task OversizedChunkedRequest_IsRejectedBeforeDeserialization()
    {
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/artwork-views")
        {
            Content = new ChunkedContent(new string(' ', 1500) + "{\"slug\":\"market-at-noon-0\"}")
        };

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.RequestEntityTooLarge, response.StatusCode);
    }

    [Fact]
    public async Task NonJsonRequest_IsRejected()
    {
        var client = factory.CreateClient();
        using var content = new StringContent("market-at-noon-0");

        var response = await client.PostAsync("/api/artwork-views", content);

        Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
    }

    [Fact]
    public async Task Views_AreIncrementedAndRankedFromPostgres()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEST_DATABASE_CONNECTION")))
        {
            Assert.NotEqual("true", Environment.GetEnvironmentVariable("REQUIRE_TEST_DATABASE"));
            return;
        }

        await ResetAnalyticsAsync();
        var client = factory.CreateClient();
        await RecordAsync(client, "market-at-noon-0", 2);
        await RecordAsync(client, "rainband-12", 3);

        var response = await client.GetFromJsonAsync<TopResponse>("/api/artwork-views/top?limit=2");

        Assert.NotNull(response);
        Assert.Collection(
            response.Items,
            first =>
            {
                Assert.Equal("rainband-12", first.Slug);
                Assert.Equal(3, first.ViewCount);
            },
            second =>
            {
                Assert.Equal("market-at-noon-0", second.Slug);
                Assert.Equal(2, second.ViewCount);
            });
    }

    [Fact]
    public async Task ConcurrentFirstViews_AreAtomic()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEST_DATABASE_CONNECTION")))
        {
            Assert.NotEqual("true", Environment.GetEnvironmentVariable("REQUIRE_TEST_DATABASE"));
            return;
        }

        await ResetAnalyticsAsync();
        var clients = Enumerable.Range(0, 10).Select(_ => factory.CreateClient()).ToArray();
        var responses = await Task.WhenAll(clients.Select(client =>
            client.PostAsJsonAsync("/api/artwork-views", new { slug = "drift-25" })));
        Assert.All(responses, response => Assert.Equal(HttpStatusCode.NoContent, response.StatusCode));

        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ArtworkViewsDbContext>();
        var count = await db.ArtworkViews
            .Where(view => view.ArtworkSlug == "drift-25")
            .Select(view => view.ViewCount)
            .SingleAsync();

        Assert.Equal(10, count);
    }

    private async Task ResetAnalyticsAsync()
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ArtworkViewsDbContext>();
        await db.ArtworkViews.ExecuteDeleteAsync();
    }

    private static async Task RecordAsync(HttpClient client, string slug, int count)
    {
        for (var index = 0; index < count; index++)
        {
            var response = await client.PostAsJsonAsync("/api/artwork-views", new { slug });
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }

    private sealed record TopResponse(IReadOnlyList<TopItem> Items);

    private sealed record TopItem(string Slug, long ViewCount);

    private sealed class ChunkedContent : HttpContent
    {
        private readonly string _content;

        public ChunkedContent(string content)
        {
            _content = content;
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
            await stream.WriteAsync(System.Text.Encoding.UTF8.GetBytes(_content));

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }

    }
}
