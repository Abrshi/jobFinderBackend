using jobFinder.Domain.Entities;
using jobFinderBackend.Application.Profile.DTOs;

namespace jobFinderBackend.Application.Interfaces;

public interface IUserJobPlatformRepository
{
    Task<List<UserJobSource>> GetForUserAsync(int userId);

    Task ReplaceForUserAsync(
        int userId,
        IEnumerable<UserJobPlatformSelectionDto> jobPlatforms);
}