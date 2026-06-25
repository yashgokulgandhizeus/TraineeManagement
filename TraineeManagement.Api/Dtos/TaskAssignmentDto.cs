namespace TraineeManagement.Api.Dtos;

using System;
using System.ComponentModel.DataAnnotations;

public enum AssignmentStatus
{
    Assigned,
    InProgress,
    Submitted,
    Reviewed,
    Completed
}

public class TaskAssignmentRequest
{
    [Required(ErrorMessage = "TraineeId is required")]
    public int TraineeId { get; set; }

    [Required(ErrorMessage = "MentorId is required")]
    public int MentorId { get; set; }

    [Required(ErrorMessage = "LearningTaskId is required")]
    public int LearningTaskId { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [EnumDataType(typeof(AssignmentStatus), ErrorMessage = "Only Assigned, InProgress, Submitted, Reviewed, and Completed values are allowed for status")]
    public AssignmentStatus Status { get; set; }
}

public class TaskAssignmentResponse
{
    public int Id { get; set; }
    public int TraineeId { get; set; }
    public string TraineeName { get; set; } = null!;
    public int MentorId { get; set; }
    public string MentorName { get; set; } = null!;
    public int LearningTaskId { get; set; }
    public AssignmentStatus Status { get; set; }
    public string TaskTitle { get; set; } = null!;
    public DateTime AssignedDate { get; set; }
    public DateTime DueDate { get; set; }
}
