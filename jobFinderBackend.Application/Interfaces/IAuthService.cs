using jobFinderBackend.Application.DTOs.Auth;

namespace jobFinderBackend.Application.Interfaces;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);
}