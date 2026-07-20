namespace TraineeManagement.Api.Services;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Dtos;
using TraineeManagement.Api.Exceptions;
using TraineeManagement.Api.Models;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext dbContext, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _context = dbContext;
        _config = configuration;
        _logger = logger;
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        User user = await _context.Users.FirstOrDefaultAsync(e => e.UserName.ToLower() == request.UserName.ToLower());

        bool isValid = false;

        if (user != null)
        {
            isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        }

        if (!isValid)
        {
            _logger.LogCritical("Authentication failed for username context: " + request.UserName);
            throw new UnauthorizedException("Invalid username or password.");
        }

        string token = GenerateJwtToken(user.Id, user.UserName, user.Role);

        string expiryConfig = _config["Jwt:ExpiryMinutes"] ?? "60";
        int minutes = int.Parse(expiryConfig);

        _logger.LogInformation("Login processed successfully and security payload built for: " + request.UserName);

        return new LoginResponse
        {
            Token = token,
            ExpiresIn = minutes * 60,
            User = new UserInfo
            {
                Id = user.Id,
                UserName = user.UserName,
                Role = user.Role
            }
        };
    }



    public string GenerateJwtToken(int id, string userName, UserRole role)
    {
        var jwtKey = _config["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT Secret Key is missing from the application configuration.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        string roleString = role.ToString().ToLowerInvariant();

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Role, roleString),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Double.Parse(_config["Jwt:ExpiryMinutes"] ?? "60")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
