using SharedKernal.Messaging.Commands;
using SharedKernal.Results;
using Storage.Application.Abstractions;

namespace Storage.Application.Files.Commands.CompleteMediaUpload;

internal sealed class CompleteMediaUploadCommandHandler(IFileRepository fileRepository)
    : ICommandHandler<CompleteMediaUploadCommand, Result>
{
    public async Task<Result> Handle(CompleteMediaUploadCommand request, CancellationToken cancellationToken)
    {
        var mediaAsset = await fileRepository.GetByStorageKeyAsync(request.StorageKey, cancellationToken);

        if (mediaAsset is null)
            return Error.NotFound(ErrorCodes.NotFound, $"Media asset with storage key {request.StorageKey} not found.");

        var result = mediaAsset.MarkUploadCompleted();
        if (result.IsFailure)
            return result.Error;

        await fileRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
