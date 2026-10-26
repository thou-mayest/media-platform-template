using Catalog.Application.Catalog;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Presentation.Catalog;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
    {
        var discovery = app.MapGroup("/api/v1/discovery")
            .WithTags("Discovery");

        discovery.MapGet("/home", async (ISender sender, CancellationToken ct) =>
            Results.Ok(await sender.Send(new GetHomeFeedQuery(), ct)))
            .WithSummary("Returns the homepage discovery feed.");

        discovery.MapGet("/snapshot", async (ISender sender, CancellationToken ct) =>
            Results.Ok(await sender.Send(new GetCatalogSnapshotQuery(), ct)))
            .WithSummary("Returns the browsable catalog snapshot used by the frontend.");

        var creators = app.MapGroup("/api/v1/creators")
            .WithTags("Creators");

        creators.MapGet("/", async (ISender sender, CancellationToken ct) =>
            Results.Ok((await sender.Send(new GetCatalogSnapshotQuery(), ct)).Creators))
            .WithSummary("Returns all creator cards.");

        creators.MapGet("/{slug}", async (string slug, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetCreatorProfileQuery(slug), ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
            .WithSummary("Returns a creator profile by slug.");

        var categories = app.MapGroup("/api/v1/categories")
            .WithTags("Categories");

        categories.MapGet("/", async (ISender sender, CancellationToken ct) =>
            Results.Ok((await sender.Send(new GetCatalogSnapshotQuery(), ct)).Categories))
            .WithSummary("Returns all catalog categories.");

        categories.MapGet("/all", async (ISender sender, CancellationToken ct) =>
            Results.Ok(await sender.Send(new GetAllCategoriesQuery(), ct)))
            .WithSummary("Returns all catalog categories with full details and counts.");

        categories.MapGet("/{slug}", async (string slug, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetCategoryDetailQuery(slug), ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
            .WithSummary("Returns a category page payload by slug.");

        categories.MapPost("/", async (CreateCategoryRequest request, ISender sender, CancellationToken ct) =>
        {
            var validationErrors = ValidateCategory(request.Name, request.Slug);
            if (validationErrors is not null)
            {
                return Results.ValidationProblem(validationErrors);
            }

            try
            {
                var result = await sender.Send(new CreateCategoryCommand(request.Name, request.Slug), ct);
                return Results.Created($"/api/v1/categories/{result.Slug}", result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { error = ex.Message });
            }
        })
            .WithSummary("Creates a new catalog category.");

        categories.MapPut("/{id:guid}", async (Guid id, UpdateCategoryRequest request, ISender sender, CancellationToken ct) =>
        {
            var validationErrors = ValidateCategory(request.Name, request.Slug);
            if (validationErrors is not null)
            {
                return Results.ValidationProblem(validationErrors);
            }

            try
            {
                var result = await sender.Send(new UpdateCategoryCommand(id, request.Name, request.Slug), ct);
                return result is null ? Results.NotFound() : Results.Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { error = ex.Message });
            }
        })
            .WithSummary("Updates an existing catalog category.");

        categories.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            try
            {
                var deleted = await sender.Send(new DeleteCategoryCommand(id), ct);
                return deleted ? Results.NoContent() : Results.NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
            .WithSummary("Deletes a catalog category, moving its videos to Uncategorized.");

        var videos = app.MapGroup("/api/v1/videos")
            .WithTags("Videos");

        videos.MapGet("/{slug}", async (string slug, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetVideoDetailQuery(slug), ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
            .WithSummary("Returns a video detail payload by slug.");

        videos.MapPut("/{id:guid}/category", async (Guid id, UpdateVideoCategoryRequest request, ISender sender, CancellationToken ct) =>
        {
            try
            {
                var result = await sender.Send(new UpdateVideoCategoryCommand(id, request.CategoryId), ct);
                return result is null ? Results.NotFound() : Results.Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
            .WithSummary("Changes the category of a video.");

        videos.MapPut("/{id:guid}/tags", async (Guid id, UpdateVideoTagsRequest request, ISender sender, CancellationToken ct) =>
        {
            try
            {
                var result = await sender.Send(new UpdateVideoTagsCommand(id, request.TagIds), ct);
                return result is null ? Results.NotFound() : Results.Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
            .WithSummary("Updates the tags assigned to a video.");

        var tags = app.MapGroup("/api/v1/tags")
            .WithTags("Tags");

        tags.MapGet("/", async (ISender sender, CancellationToken ct) =>
            Results.Ok(await sender.Send(new GetAllTagsQuery(), ct)))
            .WithSummary("Returns all catalog tags.");

        tags.MapGet("/{slug}", async (string slug, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetTagDetailQuery(slug), ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
            .WithSummary("Returns a tag page payload by slug.");

        tags.MapPost("/", async (CreateTagRequest request, ISender sender, CancellationToken ct) =>
        {
            var validationErrors = ValidateTag(request.Name, request.Slug);
            if (validationErrors is not null)
            {
                return Results.ValidationProblem(validationErrors);
            }

            try
            {
                var result = await sender.Send(new CreateTagCommand(request.Name, request.Slug), ct);
                return Results.Created($"/api/v1/tags/{result.Slug}", result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { error = ex.Message });
            }
        })
            .WithSummary("Creates a new catalog tag.");

        tags.MapPut("/{id:guid}", async (Guid id, UpdateTagRequest request, ISender sender, CancellationToken ct) =>
        {
            var validationErrors = ValidateTag(request.Name, request.Slug);
            if (validationErrors is not null)
            {
                return Results.ValidationProblem(validationErrors);
            }

            try
            {
                var result = await sender.Send(new UpdateTagCommand(id, request.Name, request.Slug), ct);
                return result is null ? Results.NotFound() : Results.Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { error = ex.Message });
            }
        })
            .WithSummary("Updates an existing catalog tag.");

        tags.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var deleted = await sender.Send(new DeleteTagCommand(id), ct);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
            .WithSummary("Deletes a catalog tag and detaches it from all videos.");

        var search = app.MapGroup("/api/v1/search")
            .WithTags("Search");

        search.MapGet("/", async (string? q, ISender sender, CancellationToken ct) =>
            Results.Ok(await sender.Send(new SearchCatalogQuery(q ?? string.Empty), ct)))
            .WithSummary("Searches creators and videos.");

        return app;
    }

    public sealed record CreateCategoryRequest(string Name, string? Slug);

    public sealed record UpdateCategoryRequest(string Name, string? Slug);

    public sealed record CreateTagRequest(string Name, string? Slug);

    public sealed record UpdateTagRequest(string Name, string? Slug);

    public sealed record UpdateVideoCategoryRequest(Guid CategoryId);

    public sealed record UpdateVideoTagsRequest(IReadOnlyList<Guid> TagIds);

    private static Dictionary<string, string[]>? ValidateCategory(string name, string? slug)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(name))
        {
            errors["name"] = ["Category name is required."];
        }
        else if (name.Trim().Length > 120)
        {
            errors["name"] = ["Category name must be 120 characters or fewer."];
        }

        if (!string.IsNullOrWhiteSpace(slug) && slug.Trim().Length > 120)
        {
            errors["slug"] = ["Category slug must be 120 characters or fewer."];
        }

        return errors.Count == 0 ? null : errors;
    }

    private static Dictionary<string, string[]>? ValidateTag(string name, string? slug)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(name))
        {
            errors["name"] = ["Tag name is required."];
        }
        else if (name.Trim().Length > 120)
        {
            errors["name"] = ["Tag name must be 120 characters or fewer."];
        }

        if (!string.IsNullOrWhiteSpace(slug) && slug.Trim().Length > 120)
        {
            errors["slug"] = ["Tag slug must be 120 characters or fewer."];
        }

        return errors.Count == 0 ? null : errors;
    }
}
