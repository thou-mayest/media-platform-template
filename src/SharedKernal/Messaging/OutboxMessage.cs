using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Messaging;

/// <summary>
/// Represents an event message stored in the database for the Outbox Pattern.
/// </summary>
public class OutboxMessage
{
    /// <summary>
    /// Unique identifier for the outbox message.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The event type name (e.g., UserRegisteredEvent).
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The serialized event payload in JSON format.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// The UTC timestamp when the event was created.
    /// </summary>
    public DateTime OccurredOnUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The UTC timestamp when the message was successfully processed. Null if pending.
    /// </summary>
    public DateTime? ProcessedOnUtc { get; set; }

    /// <summary>
    /// Error message details if processing fails.
    /// </summary>
    public string? Error { get; set; }
}