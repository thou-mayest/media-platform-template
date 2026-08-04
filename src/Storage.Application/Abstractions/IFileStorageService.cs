namespace Storage.Application.Abstractions;

internal interface IFileStorageService
{
    Task<FileUploadResult> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);
}

internal sealed record FileUploadResult(
    string StorageProvider,
    string BucketName,
    string StorageKey,
    string Url);
