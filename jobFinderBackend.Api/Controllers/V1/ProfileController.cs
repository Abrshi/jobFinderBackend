using System.Security.Claims;
using Asp.Versioning;
using FluentValidation;
using jobFinderBackend.Application.Profile.Commands.UpdateProfileSkills;
using jobFinderBackend.Application.Profile.Commands.UpdateMyProfile;
using jobFinderBackend.Application.Profile.Commands.UpdateUserPlatforms;
using jobFinderBackend.Application.Profile.DTOs;
using jobFinderBackend.Application.Profile.Queries.GetMyProfile;
using jobFinderBackend.Application.Profile.Queries.GetJobPlatform;
using jobFinderBackend.Application.Profile.Queries.GetMySkills;
using jobFinderBackend.Application.Profile.Queries.GetSkills;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace jobFinderBackend.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/profile")]
[ApiVersion("1.0")]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<ProfileResponse>> GetProfile(
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _mediator.Send(
                new GetMyProfileQuery(), cancellationToken));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<ProfileResponse>> UpdateProfile(
        UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _mediator.Send(
                new UpdateMyProfileCommand(request), cancellationToken));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                message = "Profile data is invalid.",
                errors = ex.Errors.Select(error => new
                {
                    field = error.PropertyName,
                    message = error.ErrorMessage
                })
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (DbUpdateException)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "The profile could not be saved." });
        }
    }

    [Authorize]
    [HttpGet("skills")]
    public async Task<ActionResult<List<GetSkillsResponse>>> GetSkills()
    {
        var result = await _mediator.Send(new GetSkillsQuery());

        return Ok(result);
    }

    [Authorize]
    [HttpGet("skills/my")]
    public async Task<ActionResult<List<int>>> GetMySkills()
    {
        var result = await _mediator.Send(new GetMySkillsQuery());

        return Ok(result);
    }

    [Authorize]
    [HttpPut("skills")]
    public async Task<IActionResult> UpdateSkills(
        UpdateUserSkillsRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (
            userIdClaim is null ||
            !int.TryParse(userIdClaim.Value, out var userId)
        )
        {
            return Unauthorized(new
            {
                message = "User is not authenticated."
            });
        }

        await _mediator.Send(
            new UpdateProfileSkillsCommand(
                userId,
                request.Skills
            )
        );

        return Ok(new
        {
            message = "Skills updated successfully."
        });
    }

    [Authorize]
    [HttpGet("platforms")]
    public async Task<ActionResult<List<GetJobPlatformResponse>>> GetJobPlatform()
    {
        var result = await _mediator.Send(new GetJobPlatformQuery());

        return Ok(result);
    }

    [Authorize]
    [HttpGet("platforms/my")]
    public async Task<ActionResult<List<int>>> GetMyJobPlatform()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (
            userIdClaim is null ||
            !int.TryParse(userIdClaim.Value, out var userId)
        )
        {
            return Unauthorized(new
            {
                message = "User is not authenticated."
            });
        }

        var result = await _mediator.Send(
            new GetMyJobPlatformQuery(userId)
        );

        return Ok(result);
    }

    [Authorize]
    [HttpPut("platforms")]
    public async Task<IActionResult> UpdatePlatforms(
        UpdateUserPlatformsRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (
            userIdClaim is null ||
            !int.TryParse(userIdClaim.Value, out var userId)
        )
        {
            return Unauthorized(new
            {
                message = "User is not authenticated."
            });
        }

        try
        {
            await _mediator.Send(
                new UpdateUserPlatformsCommand(
                    userId,
                    request.PlatformIds
                )
            );

            return Ok(new
            {
                message = "Platforms updated successfully.",
                platformIds = request.PlatformIds
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}