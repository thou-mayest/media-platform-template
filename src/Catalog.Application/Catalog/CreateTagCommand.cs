using Catalog.Application.Abstractions;
using Catalog.Domain;
using SharedKernal.Messaging;

namespace Catalog.Application.Catalog;

public sealed record CreateTagCommand(string Name, string? Slug = null) : ICommand<TagDto>;

internal sealed class CreateTagCommandHandler(ICatalogRepository repository)
    : ICommandHandler<CreateTagCommand, TagDto>
{
    public async Task<TagDto> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var slug = string.IsNullOrWhiteSpace(request.Slug)
            ? SlugHelper.Slugify(request.Name)
            : SlugHelper.Slugify(request.Slug);

        var existingWithSlug = await repository.GetTagBySlugAsync(slug, cancellationToken);
        if (existingWithSlug is not null)
        {
            throw new InvalidOperationException($"A tag with slug '{slug}' already exists.");
        }

        var existingWithName = await repository.GetTagByNameAsync(request.Name, cancellationToken);
        if (existingWithName is not null)
        {
            throw new InvalidOperationException($"A tag with name '{request.Name}' already exists.");
        }

        var tag = new Tag(request.Name, slug);
        await repository.AddTagAsync(tag, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return tag.ToTagDto();
    }
}

