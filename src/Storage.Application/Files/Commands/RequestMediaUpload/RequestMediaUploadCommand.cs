using SharedKernal.Messaging.Commands;
using SharedKernal.Results;

namespace Storage.Application.Files.Commands.RequestMediaUpload;

internal sealed record RequestMediaUploadCommand(
    string OriginalFileName,
    string ContentType,
    long FileSize) : ICommand<Result<RequestMediaUploadResult>>;

public sealed record RequestMediaUploadResult(Guid MediaAssetId, string UploadUrl);
