using jobFinderBackend.Application.Interfaces;
using MediatR;

namespace jobFinderBackend.Application.Profile.Commands.UpdateUserPlatforms;

public record UpdateUserPlatformsCommand(
    int UserId,
    List<int> PlatformIds
) : IRequest;

public class UpdateUserPlatformsCommandHandler
    : IRequestHandler<UpdateUserPlatformsCommand>
{
    private readonly IJobPlatformRepository _jobPlatformRepository;
    private readonly IUserSubscriptionRepository _userSubscriptionRepository;
    private readonly IUserJobPlatformRepository _userJobPlatformRepository;

    public UpdateUserPlatformsCommandHandler(
        IJobPlatformRepository jobPlatformRepository,
        IUserSubscriptionRepository userSubscriptionRepository,
        IUserJobPlatformRepository userJobPlatformRepository)
    {
        _jobPlatformRepository = jobPlatformRepository;
        _userSubscriptionRepository = userSubscriptionRepository;
        _userJobPlatformRepository = userJobPlatformRepository;
    }

    public async Task Handle(
        UpdateUserPlatformsCommand request,
        CancellationToken cancellationToken)
    {
        // Step 1: Get user's active subscription
        var subscription = await _userSubscriptionRepository
            .GetActiveSubscriptionByUserIdAsync(request.UserId);

        if (subscription is null)
        {
            throw new InvalidOperationException(
                "User does not have an active subscription."
            );
        }

        // Step 2: Deduplicate and validate requested platform count
        var uniquePlatformIds = request.PlatformIds
            .Distinct()
            .ToList();

        var maxSources = subscription.SubscriptionPlan.MaxJobSources;

        if (uniquePlatformIds.Count > maxSources)
        {
            throw new InvalidOperationException(
                $"Your subscription allows a maximum of {maxSources} job sources. " +
                $"You requested {uniquePlatformIds.Count} platforms."
            );
        }

        // Step 3: Validate each platform exists and is active
        if (uniquePlatformIds.Count > 0)
        {
            var allPlatforms = await _jobPlatformRepository.GetAllAsync();

            var allPlatformIds = allPlatforms
                .Select(jp => jp.Id)
                .ToHashSet();

            var activePlatformIds = allPlatforms
                .Where(jp => jp.IsActive)
                .Select(jp => jp.Id)
                .ToHashSet();

            var invalidPlatformIds = uniquePlatformIds
                .Except(allPlatformIds)
                .ToList();

            if (invalidPlatformIds.Count > 0)
            {
                throw new InvalidOperationException(
                    "One or more requested platforms do not exist."
                );
            }

            var inactivePlatformIds = uniquePlatformIds
                .Except(activePlatformIds)
                .ToList();

            if (inactivePlatformIds.Count > 0)
            {
                throw new InvalidOperationException(
                    "One or more requested platforms are not available."
                );
            }
        }

        // Step 4: Update the user's platform selection
        var platformDtos = uniquePlatformIds
            .Select(id => new jobFinderBackend.Application.Profile.DTOs.UserJobPlatformSelectionDto
            {
                JobPlatformId = id,
                AccountUrl = null,
                IsSynced = false
            })
            .ToList();

        await _userJobPlatformRepository.ReplaceForUserAsync(
            request.UserId,
            platformDtos
        );
    }
}
