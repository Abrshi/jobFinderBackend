using jobFinder.Application.Jobs.DTOs;

namespace jobFinder.Application.Jobs.Sources;

public interface IJobSource
{
    string PlatformName { get; }

    Task<IReadOnlyList<ExternalJobDto>> GetJobsAsync(
        CancellationToken cancellationToken = default);
}