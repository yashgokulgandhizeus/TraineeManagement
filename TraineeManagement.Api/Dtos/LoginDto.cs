namespace TraineeManagement.Api.Dtos;

using System.ComponentModel.DataAnnotations;

public enum UserRole
{
    Admin,
    Mentor,
    Trainee
}

public class LoginRequest
{
    [Required(ErrorMessage = "name is required")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "password is required")]
    public string Password { get; set; }
}

public class LoginResponse
{
    public string Token { get; set; }
    public int ExpiresIn { get; set; }
    public UserInfo User { get; set; }
}

public class UserInfo
{
    public int Id { get; set; } 
    public string UserName { get; set; }
    public UserRole Role { get; set; }
}
