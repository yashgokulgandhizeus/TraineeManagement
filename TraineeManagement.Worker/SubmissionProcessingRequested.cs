using System.Text.Json;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Contracts;

public class SubmissionProcessingRequested
{
    public Guid MessageId { get; set; } = Guid.NewGuid();
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
    public int SubmissionId { get; set; }
    public int FileId { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public string ContractVersion { get; set; } = "1.0.0";
}
