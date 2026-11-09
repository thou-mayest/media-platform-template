namespace Storage.Application.Abstractions;

internal interface IFileStorageService
{
    Task<FileUploadResult> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<FileUploadResult> UploadMultiPart(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken);

    Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);

    Task<string> GetPresignedUrlAsync(string storageKey, CancellationToken cancellationToken = default);

    Task<PresignedUploadResult> GetPresignedUploadUrlAsync(
        string originalFileName,
        string contentType,
        CancellationToken cancellationToken = default);
}

internal sealed record FileUploadResult(
    string StorageProvider,
    string BucketName,
    string StorageKey,
    string Url);

internal sealed record PresignedUploadResult(
    string StorageProvider,
    string BucketName,
    string StorageKey,
    string Url,
    string UploadUrl);
