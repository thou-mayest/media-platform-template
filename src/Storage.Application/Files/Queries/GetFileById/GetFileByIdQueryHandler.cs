using Microsoft.Extensions.Caching.Hybrid;
using SharedKernal.Messaging;
using SharedKernal.Results;
using Storage.Application.Abstractions;

namespace Storage.Application.Files.Queries.GetFileById;

internal sealed class GetFileByIdQueryHandler(IFileRepository fileRepository, HybridCache cache)
    : IQueryHandler<GetFileByIdQuery, Result<FileDto>>
{
    public async Task<Result<FileDto>> Handle(GetFileByIdQuery request, CancellationToken cancellationToken)
    {

        FileDto file = await cache.GetOrCreateAsync(
            FilesCacheKeys.GetFileKey(request.Id),
            async ct =>
            {
                var file = await fileRepository.GetByIdAsync(request.Id, ct);
                return file.ToDto();
            },
            tags: [FilesCacheKeys.Tag],
            cancellationToken: cancellationToken
            );

        if (file is null)
            return Error.NotFound(ErrorCodes.NotFound, $"File with id {request.Id} not found.");

        return Result.Success(file);
    }
}
