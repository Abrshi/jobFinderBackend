using System.Security.Claims;
using Asp.Versioning;
using jobFinderBackend.Application.Profile.Commands.UpdateProfileSkills;
using jobFinderBackend.Application.Profile.Commands.UpdateUserPlatforms;
using jobFinderBackend.Application.Profile.DTOs;
using jobFinderBackend.Application.Profile.Queries.GetJobPlatform;
using jobFinderBackend.Application.Profile.Queries.GetMySkills;
using jobFinderBackend.Application.Profile.Queries.GetSkills;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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