using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TraineeManagement.Api.Dtos;

public class LoginRequest
{
    [Required(ErrorMessage ="name is required")]
    public string UserName{get;set;}

    [Required(ErrorMessage ="password is required")]
    public string Password{get;set;}
}

public class LoginResponse
{
    public string Token{get;set;}

    public int ExpiresIn{get;set;}

    public UserInfo User{get;set;}
}

public class UserInfo
{
    public int Id;
    public string UserName{get;set;}
    public string Role{get;set;}
}