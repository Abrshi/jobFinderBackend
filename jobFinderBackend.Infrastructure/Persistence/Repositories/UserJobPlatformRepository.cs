using jobFinder.Domain.Entities;
using jobFinderBackend.Application.Interfaces;
using jobFinderBackend.Application.Profile.DTOs;
using jobFinderBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace jobFinderBackend.Infrastructure.Persistence.Repositories;

public class UserJobPlatformRepository : IUserJobPlatformRepository
{
    private readonly JobFinderBackendDbContext _context;

    public UserJobPlatformRepository(JobFinderBackendDbContext context)
    {
        _context = context;
    }

    public async Task ReplaceForUserAsync(int userId, IEnumerable<UserJobPlatformSelectionDto> jobPlatforms)
    {
        var existingPlatforms = await _context.UserJobSources
            .Where(ujs => ujs.UserId == userId)
            .ToListAsync();

        if (existingPlatforms.Count > 0)
        {
            _context.UserJobSources.RemoveRange(existingPlatforms);
        }

        var userJobSources = jobPlatforms
            .Select(jp => new UserJobSource
            {
                UserId = userId,
                JobPlatformId = jp.JobPlatformId,
                IsActive = jp.IsSynced,
                SelectedAt = DateTime.UtcNow,
                LastSyncDate = null
            })
            .ToList();

        if (userJobSources.Count > 0)
        {
            await _context.UserJobSources.AddRangeAsync(userJobSources);
        }

        await _context.SaveChangesAsync();
    }
}