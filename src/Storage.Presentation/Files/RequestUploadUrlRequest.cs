namespace Storage.Presentation.Files;

public sealed record RequestUploadUrlRequest(
    string OriginalFileName,
    string ContentType,
    long FileSize);
