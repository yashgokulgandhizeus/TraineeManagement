namespace TraineeManagement.Api.Dtos;

public class JobStatusResponse
{
    public int JobId { get; set; }
    public Guid TrackingIdentifier { get; set; }
    public int AssociatedFileId { get; set; }
    public string CurrentState { get; set; } = string.Empty;
    public int ExecutionAttempts { get; set; }
    public string? FailureLogs { get; set; }
    public object Timestamps { get; set; } = null!;
}
