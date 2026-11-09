using SharedKernal.Messaging.Commands;
using SharedKernal.Results;
using Storage.Application.Abstractions;
using Storage.Domain;

namespace Storage.Application.Files.Commands.RequestMediaUpload;

internal sealed class RequestMediaUploadCommandHandler(
    IFileStorageService fileStorageService,
    IFileRepository fileRepository)
    : ICommandHandler<RequestMediaUploadCommand, Result<RequestMediaUploadResult>>
{
    public async Task<Result<RequestMediaUploadResult>> Handle(
        RequestMediaUploadCommand request, CancellationToken cancellationToken)
    {
        if (request.FileSize <= 0)
            return Error.Validation("File.Empty", "File cannot be empty.");

        if (request.FileSize > 100 * 1024 * 1024)
            return Error.Validation("File.TooLarge", "File size exceeds 100 MB limit."); // TODO: change later

        var presigned = await fileStorageService.GetPresignedUploadUrlAsync(
            request.OriginalFileName,
            request.ContentType,
            cancellationToken);

        var mediaAssetResult = MediaAsset.Create(
            request.OriginalFileName,
            request.ContentType,
            request.FileSize,
            presigned.StorageProvider,
            presigned.BucketName,
            presigned.StorageKey,
            presigned.Url);

        if (mediaAssetResult.IsFailure)
            return mediaAssetResult.Error;

        var asset = mediaAssetResult.Value;

        await fileRepository.AddAsync(asset, cancellationToken);
        await fileRepository.SaveChangesAsync(cancellationToken);

        return new RequestMediaUploadResult(asset.Id, presigned.UploadUrl);
    }
}
