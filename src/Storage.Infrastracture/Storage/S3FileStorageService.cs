using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Storage.Application.Abstractions;

namespace Storage.Infrastracture.Storage;

internal sealed class S3FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3Options _options;

    public S3FileStorageService(IOptions<S3Options> options)
    {
        _options = options.Value;

        var config = new AmazonS3Config
        {
            RegionEndpoint = !string.IsNullOrEmpty(_options.Region)
                ? RegionEndpoint.GetBySystemName(_options.Region)
                : RegionEndpoint.EUWest1
        };

        if (!string.IsNullOrEmpty(_options.ServiceURL))
        {
            config.ServiceURL = _options.ServiceURL;
            config.ForcePathStyle = _options.ForcePathStyle;
        }

        _s3Client = new AmazonS3Client(_options.AccessKey, _options.SecretKey, config);
    }

    public async Task<FileUploadResult> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var storageKey = $"uploads/{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid():N}{Path.GetExtension(fileName)}";

        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = storageKey,
            InputStream = fileStream,
            ContentType = contentType,
            AutoCloseStream = false
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);

        var url = !string.IsNullOrEmpty(_options.ServiceURL)
            ? $"{_options.ServiceURL}/{_options.BucketName}/{storageKey}"
            : $"https://{_options.BucketName}.s3.{_options.Region}.amazonaws.com/{storageKey}";

        return new FileUploadResult(
            string.IsNullOrEmpty(_options.ServiceURL) ? "AWS-S3" : "S3-Compatible",
            _options.BucketName,
            storageKey,
            url);
    }
}
