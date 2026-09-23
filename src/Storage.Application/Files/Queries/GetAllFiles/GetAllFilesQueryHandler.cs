using Microsoft.Extensions.Caching.Hybrid;
using SharedKernal.Messaging;
using SharedKernal.Results;
using Storage.Application.Abstractions;

namespace Storage.Application.Files.Queries.GetAllFiles;

internal sealed class GetAllFilesQueryHandler(IFileRepository fileRepository, HybridCache cache)
    : IQueryHandler<GetAllFilesQuery, Result<IReadOnlyList<FileDto>>>
{
    public async Task<Result<IReadOnlyList<FileDto>>> Handle(
        GetAllFilesQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<FileDto> files = await cache.GetOrCreateAsync(
            FilesCacheKeys.AllFiles,
            async (ct) =>
            {
                var result = await fileRepository.ListAllAsync(ct);
                return result
                    .Select(f => f.ToDto())
                    .ToList();
            },
            tags: [FilesCacheKeys.Tag],
            cancellationToken: cancellationToken
            );

        if (files is null || files.Count == 0)
        {
            return Error.NotFound(ErrorCodes.NotFound, "list of files is empty");
        }

        return Result.Success(files);
    }
}
