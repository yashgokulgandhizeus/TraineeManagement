namespace TraineeManagement.Api.Dtos;

using System;
using System.ComponentModel.DataAnnotations;

public enum TaskStatus
{
    Draft,
    Published,
    Closed
}

public class LearningTaskRequest
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; }

    [Required(ErrorMessage = "ExpectedTechStack is required")]
    [MaxLength(500, ErrorMessage = "Tech stack description cannot exceed 500 characters")]
    public string ExpectedTechStack { get; set; }

    [Required(ErrorMessage = "DueDate is required")]
    public DateTime DueDate { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [EnumDataType(typeof(TaskStatus), ErrorMessage = "Only Draft, Published, and Closed values are allowed for Status")]
    public TaskStatus Status { get; set; }
}

public class LearningTaskResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Desciption { get; set; }
    public string ExpectedTechStack { get; set; }
    public DateTime DueDate { get; set; }
    public TaskStatus Status { get; set; }
}
