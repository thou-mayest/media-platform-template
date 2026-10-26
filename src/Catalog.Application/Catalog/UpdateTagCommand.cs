using Catalog.Application.Abstractions;
using Catalog.Domain;
using SharedKernal.Messaging;

namespace Catalog.Application.Catalog;

public sealed record UpdateTagCommand(Guid Id, string Name, string? Slug = null) : ICommand<TagDto?>;

internal sealed class UpdateTagCommandHandler(ICatalogRepository repository)
    : ICommandHandler<UpdateTagCommand, TagDto?>
{
    public async Task<TagDto?> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await repository.GetTagByIdAsync(request.Id, cancellationToken);
        if (tag is null)
        {
            return null;
        }

        var slug = string.IsNullOrWhiteSpace(request.Slug)
            ? SlugHelper.Slugify(request.Name)
            : SlugHelper.Slugify(request.Slug);

        if (tag.Slug.Equals(Tag.DefaultSlug, StringComparison.OrdinalIgnoreCase) &&
            !slug.Equals(Tag.DefaultSlug, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("The default untagged tag slug cannot be changed.");
        }

        var existingWithSlug = await repository.GetTagBySlugAsync(slug, cancellationToken);
        if (existingWithSlug is not null && existingWithSlug.Id != tag.Id)
        {
            throw new InvalidOperationException($"A tag with slug '{slug}' already exists.");
        }

        var existingWithName = await repository.GetTagByNameAsync(request.Name, cancellationToken);
        if (existingWithName is not null && existingWithName.Id != tag.Id)
        {
            throw new InvalidOperationException($"A tag with name '{request.Name}' already exists.");
        }

        tag.Update(request.Name, slug);
        repository.UpdateTag(tag);
        await repository.SaveChangesAsync(cancellationToken);

        return tag.ToTagDto();
    }
}

