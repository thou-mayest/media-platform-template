using SharedKernal.Messaging.Commands;
using SharedKernal.Results;

namespace Posts.Application.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(string? Name) : ICommand<Result<Guid>>;
