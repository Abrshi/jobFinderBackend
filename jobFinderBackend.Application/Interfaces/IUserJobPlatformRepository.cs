using jobFinderBackend.Application.Profile.DTOs;

namespace jobFinderBackend.Application.Interfaces;

public interface IUserJobPlatformRepository
{
    Task ReplaceForUserAsync(int userId, IEnumerable<UserJobPlatformSelectionDto> jobPlatforms);
}
