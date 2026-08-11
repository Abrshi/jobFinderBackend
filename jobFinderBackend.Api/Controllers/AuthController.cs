using jobFinder.Application.DTOs.Auth;
using jobFinder.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace jobFinderBackend.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register(
        RegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);

            return CreatedAtAction(
                nameof(Register),
                new { id = response.Id },
                response
            );
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }
}