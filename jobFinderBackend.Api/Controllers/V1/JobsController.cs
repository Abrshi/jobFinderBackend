using Asp.Versioning;
using jobFinderBackend.Application.Jobs.DTOs;
using jobFinderBackend.Application.Jobs.Queries.GetJobById;
using jobFinderBackend.Application.Jobs.Queries.GetJobs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace jobFinderBackend.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/jobs")]
[ApiVersion("1.0")]
public class JobsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<GetJobsResponse>> GetJobs(
        [FromQuery] JobFilterParams filters)
    {
        var result = await _mediator.Send(new GetJobsQuery(filters));

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<JobListItemResponse>> GetJobById(int id)
    {
        var result = await _mediator.Send(new GetJobByIdQuery(id));

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
