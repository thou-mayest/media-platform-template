using SharedKernal.Messaging.Commands;
using SharedKernal.Results;

namespace Posts.Application.Categories.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(Guid Id, string? Name) : ICommand<Result>;
