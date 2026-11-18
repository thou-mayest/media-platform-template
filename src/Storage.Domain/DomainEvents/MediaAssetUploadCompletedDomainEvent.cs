using SharedKernal.Messaging.DomainEvents;

namespace Storage.Domain.DomainEvents;

public sealed record MediaAssetUploadCompletedDomainEvent(Guid Guid) : IDomainEvent;
