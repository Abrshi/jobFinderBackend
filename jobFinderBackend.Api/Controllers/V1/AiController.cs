using Asp.Versioning;
using FluentValidation;
using jobFinderBackend.Application.Applications.Commands.GenerateApplicationDocuments;
using jobFinderBackend.Application.Applications.DTOs;
using jobFinderBackend.Application.Applications.Queries.GetGeneratedDocuments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace jobFinderBackend.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/ai")]
[ApiVersion("1.0")]
[Authorize]
public class AiController : ControllerBase
{
    private readonly IMediator _mediator;

    public AiController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("generate-application-documents")]
    public async Task<ActionResult<GeneratedDocumentsResponse>> GenerateDocuments(
        GenerateApplicationDocumentsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(
                new GenerateApplicationDocumentsCommand(request.JobId),
                cancellationToken);

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }
        catch (ForbiddenAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                message = "The request is invalid.",
                errors = ex.Errors.Select(error => new
                {
                    field = error.PropertyName,
                    message = error.ErrorMessage
                })
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (DbUpdateException)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "The generated documents could not be saved." });
        }
        catch (HttpRequestException)
        {
            return StatusCode(
                StatusCodes.Status502BadGateway,
                new { message = "The document generation service is unavailable." });
        }
        catch (InvalidOperationException)
        {
            return StatusCode(
                StatusCodes.Status502BadGateway,
                new { message = "The document generation service returned an invalid response." });
        }
    }

    [HttpGet("generated-documents/{jobId:int}")]
    public async Task<ActionResult<GeneratedDocumentsResponse>> GetGeneratedDocuments(
        int jobId,
        CancellationToken cancellationToken)
    {
        if (jobId <= 0)
        {
            return BadRequest(new { message = "Job ID must be greater than zero." });
        }

        try
        {
            var result = await _mediator.Send(
                new GetGeneratedDocumentsQuery(jobId),
                cancellationToken);

            return result is null
                ? NotFound(new { message = "Generated documents were not found." })
                : Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "User is not authenticated." });
        }
        catch (InvalidOperationException)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "The generated documents could not be read." });
        }
    }
}