using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Dtos;
using TraineeManagement.Api.Services;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController:ControllerBase
{

    private readonly IAuthService _service;
    public AuthController(IAuthService authService)
    {
        _service=authService;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> login(LoginRequest Request)
    {
        
        var result = await _service.Login(Request);
        if(result==null)
        return Unauthorized(new{Message="Username or Password is incorrect."});

        else
        return Ok(result);
    } 
}