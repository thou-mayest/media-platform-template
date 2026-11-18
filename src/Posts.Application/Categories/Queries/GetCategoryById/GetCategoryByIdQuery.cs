using SharedKernal.Messaging.Queries;
using SharedKernal.Results;

namespace Posts.Application.Categories.Queries.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid Id) : IQuery<Result<CategoryDto>>;
