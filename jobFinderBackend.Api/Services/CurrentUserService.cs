using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using jobFinderBackend.Application.Interfaces;

namespace jobFinderBackend.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(userId, out var id)
                ? id
                : null;
        }
    }

    public string? Role =>
        _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.Role);
}