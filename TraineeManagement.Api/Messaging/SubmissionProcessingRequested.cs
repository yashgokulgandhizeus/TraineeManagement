using System;

namespace TraineeManagement.Api.Messaging;

public record SubmissionProcessingRequested
{
    public Guid MessageId { get; init; } = Guid.NewGuid();
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public int SubmissionId { get; init; }
    public int FileId { get; init; }
    public DateTime RequestedAt { get; init; } = DateTime.UtcNow;
    public string ContractVersion { get; init; } = "1.0.0";
}
