using SkillEntity = jobFinder.Domain.Entities.Skill;

namespace jobFinderBackend.Application.Interfaces;

public interface ISkillRepository
{
    Task<List<SkillEntity>> GetAllAsync();
}
