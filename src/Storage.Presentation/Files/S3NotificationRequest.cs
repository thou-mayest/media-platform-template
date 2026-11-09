namespace Storage.Presentation.Files;

public sealed record S3NotificationRequest(IReadOnlyList<S3NotificationRecord> Records);

public sealed record S3NotificationRecord(string EventName, S3NotificationEntity S3);

public sealed record S3NotificationEntity(S3NotificationBucket Bucket, S3NotificationObject Object);

public sealed record S3NotificationBucket(string Name);

public sealed record S3NotificationObject(string Key);
