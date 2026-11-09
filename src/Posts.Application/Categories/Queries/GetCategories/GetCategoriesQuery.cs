using SharedKernal.Messaging.Queries;
using SharedKernal.Results;

namespace Posts.Application.Categories.Queries.GetCategories;

public sealed record GetCategoriesQuery : IQuery<Result<IReadOnlyList<CategoryDto>>>;
