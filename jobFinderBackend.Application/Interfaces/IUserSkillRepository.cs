using jobFinderBackend.Application.Profile.DTOs;

namespace jobFinderBackend.Application.Interfaces;

public interface IUserSkillRepository
{
    Task ReplaceForUserAsync(int userId, IEnumerable<UserSkillSelectionDto> skills);
}
