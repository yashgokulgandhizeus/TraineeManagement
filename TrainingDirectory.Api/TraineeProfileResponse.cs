using System;

namespace TrainingDirectory.Api;

public record TraineeProfileResponse
{
    public int TraineeId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string TierCode { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string Track { get; init; } = "Backend Engineering";
}
