using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Models;

public class SubmissionFile
{
    [Key]
    public int Id { get; set; }

    public string? OriginalFileName { get; set; }

    public string? StoredFileName { get; set; }

    public string? ContentType { get; set; }
    public string? Checksum { get; set; }
    public string? UploadedByUser { get; set; }
    public int? UploadedByUserId { get; set; }

    public long? FileSize { get; set; }

    public int? SubmissionId { get; set; }
    public DateTime UploadedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public Submission? Submission { get; set; }
}