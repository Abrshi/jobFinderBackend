// imapper le response metekem and
// exeption global mehin alebet

using Asp.Versioning;
using jobFinderBackend.Application.DTOs.Auth;
using jobFinderBackend.Application.Users.Commands;
using jobFinderBackend.Application.Users.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace jobFinderBackend.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register(
        RegisterRequest request)
    {
        try
        {
            var command = new RegisterUserCommand(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password
            );

            var user = await _mediator.Send(command);

            var response = new RegisterResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Message = "Registration successful."
            };

            return CreatedAtAction(
                nameof(Register),
                new { id = user.Id },
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

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request)
    {
        try
        {
            var command = new LoginUserCommand(
                request.Email,
                request.Password
            );

            var result = await _mediator.Send(command);

            // Store JWT in HttpOnly cookie. Work well for localhost and browser cookie restrictions.
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(1),
                Path = "/",
                IsEssential = true
            };

            Response.Cookies.Append("auth_token", result.Token, cookieOptions);

            // Never return the JWT in the response body.
            var response = new LoginResponse
            {
                Id = result.Id,
                Email = result.Email,
                Role = result.Role,
                Message = "Login successful.",
                Subscription = result.Subscription
            };

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message
            });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<GetCurrentUserResponse>> Me()
    {
        try
        {
            var result = await _mediator.Send(new GetCurrentUserQuery());
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new
            {
                message = "User is not authenticated."
            });
        }
    }
}