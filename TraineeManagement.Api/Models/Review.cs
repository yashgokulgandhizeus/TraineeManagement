using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Dtos;

namespace TraineeManagement.Api.Models;

[Index(nameof(SubmissionId), Name = "IX_Reviews_SubmissionId")]
[Index(nameof(MentorId), Name = "IX_Reviews_MentorId")]
public class Review
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int SubmissionId { get; set; }

    [ForeignKey(nameof(SubmissionId))]
    public Submission Submission { get; set; }

    public int MentorId { get; set; }

    [ForeignKey(nameof(MentorId))]
    public Mentor Mentor { get; set; }

    public string Feedback { get; set; }

    public int Score { get; set; }

    public ReviewStatus ReviewStatus { get; set; }

    public DateTime ReviewedDate { get; set; }

    public Review()
    {
        ReviewedDate = DateTime.UtcNow;
    }
}
