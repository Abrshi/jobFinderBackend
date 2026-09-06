namespace jobFinderBackend.Infrastructure.Jobd;

public interface IJobSyncService
{
    Task<JobSyncResultDto> SynchronizeJobsAsync(CancellationToken cancellationToken = default);
}

public record JobSyncResultDto(
    int NewJobs,
    int UpdatedJobs,
    int ClassifiedJobs,
    int SkippedJobs,
    int TotalReceived);
