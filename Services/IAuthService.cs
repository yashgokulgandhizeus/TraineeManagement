namespace TraineeManagement.Api.Services;

using TraineeManagement.Api.Dtos;

public interface IAuthService
{
   Task<LoginResponse> Login(LoginRequest Request);
}