namespace TraineeManagement.Api.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Dtos;
using TraineeManagement.Api.Models;

public class AuthService : IAuthService
{

    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext dbContext,IConfiguration configuration)
    {
        _context=dbContext;
        _config=configuration;
    }
    public async Task<LoginResponse> Login(LoginRequest Request)
    {
        User user=await _context.Users.FirstOrDefaultAsync(e=>e.UserName==Request.UserName);

        if(user==null)
        return null;

        bool IsValid=BCrypt.Net.BCrypt.Verify(Request.Password,user.PasswordHash);

        if(!IsValid)
        return null;

        string token=GenerateJwtToken(user.Id,user.UserName,user.Role);

        int minutes=int.Parse(_config["jwt:ExpiryMinutes"]);

        return new LoginResponse
        {
            Token=token,
            ExpiresIn=minutes,
            User=new UserInfo
            {
              Id=user.Id,
              UserName=user.UserName,
              Role=user.Role
            }
        };
        
    }

    public string GenerateJwtToken(int Id,string UserName,string Role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
            new Claim(ClaimTypes.Name, UserName),
            new Claim(ClaimTypes.Role, Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["jwt:Issuer"],
            audience: _config["jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Double.Parse(_config["jwt:ExpiryMinutes"])),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }
}