using SharedKernal.Messaging.DomainEvents;

namespace Storage.Domain.DomainEvents;

public sealed record MediaAssetUploadFailedDomainEvent(Guid Guid) : IDomainEvent;
