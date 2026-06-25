using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TraineeManagement.Api.Models;

public enum JobStatus
{
    Queued,
    Processing,
    Completed,
    Failed
}

[Index(nameof(CorrelationId), IsUnique = true, Name = "IX_ProcessingJobs_CorrelationId")]
[Index(nameof(FileId), Name = "IX_ProcessingJobs_FileId")]
public class ProcessingJob
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public Guid CorrelationId { get; set; }

    [Required]
    public int FileId { get; set; }

    [Required]
    public JobStatus Status { get; set; }

    public int Attempts { get; set; }

    public string? ErrorSummary { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
