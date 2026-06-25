namespace TraineeManagement.Api.Dtos;

using System;
using System.ComponentModel.DataAnnotations;

public enum ReviewStatus
{
    Accepted,
    ChangesRequired,
    Rejected
}

public class ReviewRequest
{
    [Required(ErrorMessage = "SubmissionId is required")]
    public int SubmissionId { get; set; }

    [Required(ErrorMessage = "Feedback is required")]
    public string Feedback { get; set; }

    [Required(ErrorMessage = "Score is required")]
    [Range(0, 10, ErrorMessage = "Score must be a number between 0 and 10")]
    public int Score { get; set; }

    [Required(ErrorMessage = "ReviewStatus is required")]
    [EnumDataType(typeof(ReviewStatus), ErrorMessage = "Only Accepted, ChangesRequired, and Rejected values are allowed for Status")]
    public ReviewStatus ReviewStatus { get; set; }
}

public class ReviewResponse
{
    public int Id { get; set; }

    public int SubmissionId { get; set; }

    public int MentorId { get; set; }

    public string MentorName { get; set; }

    public string Feedback { get; set; }

    public int Score { get; set; }

    public ReviewStatus ReviewStatus { get; set; }

    public DateTime ReviewedDate { get; set; }
}
