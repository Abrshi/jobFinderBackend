using jobFinder.Domain.Entities;
using jobFinderBackend.Application.Interfaces;
using jobFinderBackend.Application.Profile.DTOs;
using jobFinderBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace jobFinderBackend.Infrastructure.Persistence.Repositories;

public class UserSkillRepository : IUserSkillRepository
{
    private readonly JobFinderBackendDbContext _context;

    public UserSkillRepository(JobFinderBackendDbContext context)
    {
        _context = context;
    }

    public async Task ReplaceForUserAsync(int userId, IEnumerable<UserSkillSelectionDto> skills)
    {
        var existingSkills = await _context.UserSkills
            .Where(us => us.UserId == userId)
            .ToListAsync();

        if (existingSkills.Count > 0)
        {
            _context.UserSkills.RemoveRange(existingSkills);
        }

        var userSkills = skills
            .Select(s => new UserSkill
            {
                UserId = userId,
                SkillId = s.SkillId,
                Level = s.Level,
                YearsOfExperience = s.YearsOfExperience
            })
            .ToList();

        if (userSkills.Count > 0)
        {
            await _context.UserSkills.AddRangeAsync(userSkills);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<int>> GetUserSkillIdsAsync(int userId)
    {
        return await _context.UserSkills
            .Where(us => us.UserId == userId)
            .Select(us => us.SkillId)
            .Distinct()
            .ToListAsync();
    }
}
