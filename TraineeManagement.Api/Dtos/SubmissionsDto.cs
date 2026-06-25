namespace TraineeManagement.Api.Dtos;

using System;
using System.ComponentModel.DataAnnotations;

public enum SubmissionStatus
{
    Submitted,
    ReSubmitted
}

public class SubmissionRequest
{
    [Required(ErrorMessage = "TaskAssignmentId is required")]
    public int TaskAssignmentId { get; set; }

    [Required(ErrorMessage = "SubmissionUrl is required")]
    public string SubmissionUrl { get; set; }

    [Required(ErrorMessage = "Notes is required")]
    public string Notes { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [EnumDataType(typeof(SubmissionStatus), ErrorMessage = "Only Submitted and ReSubmitted are allowed for status")]
    public SubmissionStatus Status { get; set; }
}

public class SubmissionResponse
{
    public int Id { get; set; }

    public int TaskAssignmentId { get; set; }

    public string SubmissionUrl { get; set; }

    public string Notes { get; set; }

    public SubmissionStatus Status { get; set; }

    public DateTime SubmittedDate { get; set; }
}
