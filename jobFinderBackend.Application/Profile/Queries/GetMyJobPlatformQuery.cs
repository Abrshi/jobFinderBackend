using jobFinderBackend.Application.Interfaces;
using MediatR;

namespace jobFinderBackend.Application.Profile.Queries.GetJobPlatform;

public record GetMyJobPlatformQuery(int UserId) : IRequest<List<int>>;

public class GetMyJobPlatformQueryHandler
    : IRequestHandler<GetMyJobPlatformQuery, List<int>>
{
    private readonly IUserJobPlatformRepository _userJobPlatformRepository;

    public GetMyJobPlatformQueryHandler(
        IUserJobPlatformRepository userJobPlatformRepository)
    {
        _userJobPlatformRepository = userJobPlatformRepository;
    }

    public async Task<List<int>> Handle(
        GetMyJobPlatformQuery request,
        CancellationToken cancellationToken)
    {
        var jobPlatforms = await _userJobPlatformRepository
            .GetForUserAsync(request.UserId);

        return jobPlatforms
            .Select(jp => jp.JobPlatformId)
            .ToList();
    }
}