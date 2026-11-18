using SharedKernal.Messaging.Commands;
using SharedKernal.Results;
using Storage.Application.Abstractions;

namespace Storage.Application.Files.Commands.FailMediaUpload;

internal sealed class FailMediaUploadCommandHandler(IFileRepository fileRepository)
    : ICommandHandler<FailMediaUploadCommand, Result>
{
    public async Task<Result> Handle(FailMediaUploadCommand request, CancellationToken cancellationToken)
    {
        var mediaAsset = await fileRepository.GetByStorageKeyAsync(request.StorageKey, cancellationToken);

        if (mediaAsset is null)
            return Error.NotFound(ErrorCodes.NotFound, $"Media asset with storage key {request.StorageKey} not found.");

        var result = mediaAsset.MarkUploadFailed();
        if (result.IsFailure)
            return result.Error;

        await fileRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
