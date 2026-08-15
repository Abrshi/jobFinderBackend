using jobFinderBackend.Application.Interfaces;
using jobFinderBackend.Application.Profile.DTOs;
using MediatR;

namespace jobFinderBackend.Application.Profile.Queries.GetJobPlatform;

public record GetJobPlatformQuery : IRequest<List<GetJobPlatformResponse>>;

public class GetJobPlatformQueryHandler
    : IRequestHandler<GetJobPlatformQuery, List<GetJobPlatformResponse>>
{
    private readonly IJobPlatformRepository _jobPlatformRepository;

    public GetJobPlatformQueryHandler(IJobPlatformRepository jobPlatformRepository)
    {
        _jobPlatformRepository = jobPlatformRepository;
    }

    public async Task<List<GetJobPlatformResponse>> Handle(
        GetJobPlatformQuery request,
        CancellationToken cancellationToken)
    {
        var JobPlatform = await _jobPlatformRepository.GetAllAsync();

        return JobPlatform
            .Select(j => new GetJobPlatformResponse
            {
                Id = j.Id,
                Name = j.Name,
                Website= j.Website,
                Logo = j.Logo,
                SourceType = j.SourceType,
                IsActive = j.IsActive
            })
            .ToList();
    }
}
