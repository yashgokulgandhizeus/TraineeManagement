namespace TraineeManagement.Api.Dtos;

using System;
using System.ComponentModel.DataAnnotations;

public enum MentorStatus
{
    Active,
    Inactive
}

public class MentorRequest
{
    [Required(ErrorMessage = "FirstName is required")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "LastName is required")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Not a Valid Email Address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Experties is required")]
    public string Experties { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [EnumDataType(typeof(MentorStatus), ErrorMessage = "Only Active and Inactive values are allowed for Status")]
    public MentorStatus Status { get; set; }
}

public class MentorResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Experties { get; set; }
    public MentorStatus Status { get; set; }
}
