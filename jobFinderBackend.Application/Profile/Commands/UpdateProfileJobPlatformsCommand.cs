using jobFinderBackend.Application.Interfaces;
using jobFinderBackend.Application.Profile.DTOs;
using MediatR;

namespace jobFinderBackend.Application.Profile.Commands.UpdateProfileJobPlatforms;

public record UpdateProfileJobPlatformsCommand(
    int UserId,
    List<UserJobPlatformSelectionDto> JobPlatforms
) : IRequest;

public class UpdateProfileJobPlatformsCommandHandler
    : IRequestHandler<UpdateProfileJobPlatformsCommand>
{
    private readonly IJobPlatformRepository _jobPlatformRepository;
    private readonly IUserJobPlatformRepository _userJobPlatformRepository;

    public UpdateProfileJobPlatformsCommandHandler(
        IJobPlatformRepository jobPlatformRepository,
        IUserJobPlatformRepository userJobPlatformRepository)
    {
        _jobPlatformRepository = jobPlatformRepository;
        _userJobPlatformRepository = userJobPlatformRepository;
    }

    public async Task Handle(
        UpdateProfileJobPlatformsCommand request,
        CancellationToken cancellationToken)
    {
        var selectedPlatformIds = request.JobPlatforms
            .Select(jp => jp.JobPlatformId)
            .Distinct()
            .ToList();

        if (selectedPlatformIds.Count == 0)
        {
            await _userJobPlatformRepository.ReplaceForUserAsync(request.UserId, []);
            return;
        }

        var validPlatformIds = (await _jobPlatformRepository.GetAllAsync())
            .Select(jp => jp.Id)
            .ToHashSet();

        var invalidPlatformIds = selectedPlatformIds
            .Except(validPlatformIds)
            .ToList();

        if (invalidPlatformIds.Count > 0)
        {
            throw new InvalidOperationException(
                "One or more selected job platforms are invalid."
            );
        }

        await _userJobPlatformRepository.ReplaceForUserAsync(
            request.UserId,
            request.JobPlatforms
        );
    }
}
