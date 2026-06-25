using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Dtos;

namespace TraineeManagement.Api.Models;

[Index(nameof(TaskAssignmentId), Name = "IX_Submissions_TaskAssignmentId")]
public class Submission
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int TaskAssignmentId { get; set; }

    [ForeignKey(nameof(TaskAssignmentId))]
    public TaskAssignment TaskAssignment { get; set; }

    public string SubmissionUrl { get; set; }

    public string Notes { get; set; }

    public SubmissionStatus Status { get; set; }

    public DateTime SubmittedDate { get; set; }
    
    public ICollection<SubmissionFile>? Files { get; set; } = new List<SubmissionFile>();
    
    public Submission()
    {
        SubmittedDate = DateTime.UtcNow;
    }
}
