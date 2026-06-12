namespace TraineeManagement.Api.Dtos;
using System.ComponentModel.DataAnnotations;

using System;
using System.ComponentModel.DataAnnotations;

public class LearningTaskRequest
{
    [Required(ErrorMessage = "Title is required")] // Fixed error message
    public string Title { get; set; }

    [Required(ErrorMessage = "Description is required")] // Fixed error message
    public string Description { get; set; }

    [Required(ErrorMessage = "ExpectedTechStack is required")] // Fixed error message
    [MaxLength(500, ErrorMessage = "Tech stack description cannot exceed 500 characters")] // Replaced [EmailAddress] with a useful constraint
    public string ExpectedTechStack { get; set; }

    [Required(ErrorMessage = "DueDate is required")] // Fixed error message
    public DateTime DueDate { get; set; }

    [AllowedValues("Draft", "Published", "Closed", ErrorMessage = "Only Draft, Published, and Closed values are allowed for Status")]
    public string Status { get; set; }
}


public class LearningTaskResponse
{
    public int Id{get;set;}
    public string Title{get;set;}
    public string Desciption{get;set;}
    public string ExpectedTechStack{get;set;}
    public DateTime DueDate{get;set;}
    public string Status{get;set;}

}