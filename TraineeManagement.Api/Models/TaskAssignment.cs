using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Dtos;

namespace TraineeManagement.Api.Models;

[Index(nameof(TraineeId), Name = "IX_TaskAssignments_TraineeId")]
[Index(nameof(MentorId), Name = "IX_TaskAssignments_MentorId")]
[Index(nameof(LearningTaskId), Name = "IX_TaskAssignments_LearningTaskId")]
public class TaskAssignment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int TraineeId { get; set; }
    public int MentorId { get; set; }
    public int LearningTaskId { get; set; }

    [ForeignKey(nameof(TraineeId))]
    public Trainee Trainee { get; set; }

    [ForeignKey(nameof(MentorId))]
    public Mentor Mentor { get; set; }

    [ForeignKey(nameof(LearningTaskId))]
    public LearningTask LearningTask { get; set; }

    public AssignmentStatus Status { get; set; }

    public DateTime AssignedDate { get; set; }

    public DateTime DueDate { get; set; }

    public TaskAssignment()
    {
        AssignedDate = DateTime.UtcNow;
    }
}
