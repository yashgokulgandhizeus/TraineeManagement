namespace TraineeManagement.Api.Dtos;

using System.ComponentModel.DataAnnotations;

public class CreateTraineeRequest
{
    [Required(ErrorMessage ="firstname is required")]
    public string FirstName { get; set; }

    [Required(ErrorMessage ="lastname is required")]
    public string LastName { get; set; }

    [Required(ErrorMessage ="email is required")]
    [EmailAddress(ErrorMessage ="invalid email")]
    public string Email { get; set; }
    
    [Required(ErrorMessage ="techstack is required")]
    public string TechStack { get; set; }

    [Required(ErrorMessage ="status is required")]
    [AllowedValues("Active", "Inactive", "Completed",ErrorMessage ="Status must be from Active, Inactive, Completed")]
    public string Status { get; set; }
}

public class UpdateTraineeRequest
{
   [Required(ErrorMessage ="firstname is required")]
    public string FirstName { get; set; }

    [Required(ErrorMessage ="lastname is required")]
    public string LastName { get; set; }

    [Required(ErrorMessage ="email is required")]
    [EmailAddress(ErrorMessage ="invalid email")]
    public string Email { get; set; }
    
    [Required(ErrorMessage ="techstack is required")]
    public string TechStack { get; set; }

    [Required(ErrorMessage ="status is required")]
    [AllowedValues("Active", "Inactive", "Completed",ErrorMessage ="Status must be from Active, Inactive, Completed")]
    public string Status { get; set; }
}

public class TraineeResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; }

    public string LastName { get; set; }
    public string Email { get; set; }
    
    public string TechStack { get; set; }

    public string Status { get; set; }
}

public class PaginationQueryRequest
{
    public int PageNumber {get; set;}=1;
    public int PageSize {get;set;}=10;
    public String ?Search {get; set;}

    [AllowedValues("Active", "Inactive", "Completed",ErrorMessage ="Status must be from Active, Inactive, Completed")]
    public string ?Status;

}

public class PaginationQueryResponse<T>
{
    public int PageNumber {get; set;}
    public int PageSize {get; set;}
    public int TotalRecords {get; set;}
    public List<T> Data {get; set;}
}

