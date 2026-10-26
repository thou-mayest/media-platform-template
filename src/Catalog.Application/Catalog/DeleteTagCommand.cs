using Catalog.Application.Abstractions;
using Catalog.Domain;
using SharedKernal.Messaging;

namespace Catalog.Application.Catalog;

public sealed record DeleteTagCommand(Guid Id) : ICommand<bool>;

internal sealed class DeleteTagCommandHandler(ICatalogRepository repository)
    : ICommandHandler<DeleteTagCommand, bool>
{
    public async Task<bool> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await repository.GetTagByIdAsync(request.Id, cancellationToken);
        if (tag is null)
        {
            return false;
        }

        if (tag.Slug.Equals(Tag.DefaultSlug, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("The default untagged tag cannot be deleted.");
        }

        var defaultTag = await repository.GetDefaultTagAsync(cancellationToken);
        if (defaultTag is null)
        {
            defaultTag = new Tag(Tag.DefaultName, Tag.DefaultSlug);
            await repository.AddTagAsync(defaultTag, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
        }

        await repository.DeleteTagAsync(tag, defaultTag, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}

