using JobPlatform = jobFinder.Domain.Entities.JobPlatform;

namespace jobFinderBackend.Application.Interfaces;

public interface IJobPlatformRepository
{
    Task<List<JobPlatform>> GetAllAsync();
}
