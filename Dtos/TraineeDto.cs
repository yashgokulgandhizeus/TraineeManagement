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

