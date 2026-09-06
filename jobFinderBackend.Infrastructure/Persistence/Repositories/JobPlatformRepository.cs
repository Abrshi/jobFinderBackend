using jobFinder.Domain.Entities;
using jobFinderBackend.Application.Interfaces;
using jobFinderBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace jobFinderBackend.Infrastructure.Persistence.Repositories;

public class JobPlatformRepository : IJobPlatformRepository
{
    private readonly JobFinderBackendDbContext _context;

    public JobPlatformRepository(JobFinderBackendDbContext context)
    {
        _context = context;
    }

    public async Task<List<JobPlatform>> GetAllAsync()
    {
        return await _context.JobPlatforms
            .AsNoTracking()
            .ToListAsync();
    }
}
