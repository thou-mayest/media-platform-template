using SharedKernal.Messaging.Queries;
using SharedKernal.Results;

namespace Storage.Application.Files.Queries.GetFileDownloadUrl;

internal sealed record GetFileDownloadUrlQuery(Guid Id) : IQuery<Result<string>>;
