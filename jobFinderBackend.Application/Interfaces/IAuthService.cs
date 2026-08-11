using jobFinder.Application.DTOs.Auth;

namespace jobFinder.Application.Interfaces;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);
}