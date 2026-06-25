namespace TraineeManagement.Api.Dtos;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public enum TraineeStatus
{
    Active,
    Inactive,
    Completed
}

public class CreateTraineeRequest
{
    [Required(ErrorMessage = "firstname is required")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "lastname is required")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "email is required")]
    [EmailAddress(ErrorMessage = "invalid email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "techstack is required")]
    public string TechStack { get; set; }

    [Required(ErrorMessage = "status is required")]
    [EnumDataType(typeof(TraineeStatus), ErrorMessage = "Status must be from Active, Inactive, Completed")]
    public TraineeStatus Status { get; set; }
}

public class UpdateTraineeRequest
{
    [Required(ErrorMessage = "firstname is required")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "lastname is required")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "email is required")]
    [EmailAddress(ErrorMessage = "invalid email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "techstack is required")]
    public string TechStack { get; set; }

    [Required(ErrorMessage = "status is required")]
    [EnumDataType(typeof(TraineeStatus), ErrorMessage = "Status must be from Active, Inactive, Completed")]
    public TraineeStatus Status { get; set; }
}

public class TraineeResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string TechStack { get; set; }
    public TraineeStatus Status { get; set; }
}

public class PaginationQueryRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }

    [EnumDataType(typeof(TraineeStatus), ErrorMessage = "Status must be from Active, Inactive, Completed")]
    public TraineeStatus? Status { get; set; } // Changed to property and mapped to nullable enum
}

public class PaginationQueryResponse<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public List<T> Data { get; set; }
}
