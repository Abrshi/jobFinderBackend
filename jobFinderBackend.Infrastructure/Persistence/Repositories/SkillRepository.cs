using jobFinder.Domain.Entities;
using jobFinderBackend.Application.Interfaces;
using jobFinderBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace jobFinderBackend.Infrastructure.Persistence.Repositories;

public class SkillRepository : ISkillRepository
{
    private readonly JobFinderBackendDbContext _context;

    public SkillRepository(JobFinderBackendDbContext context)
    {
        _context = context;
    }

    public async Task<List<Skill>> GetAllAsync()
    {
        return await _context.Skills
            .AsNoTracking()
            .OrderBy(s => s.Category ?? string.Empty)
            .ThenBy(s => s.Name)
            .ToListAsync();
    }
}
