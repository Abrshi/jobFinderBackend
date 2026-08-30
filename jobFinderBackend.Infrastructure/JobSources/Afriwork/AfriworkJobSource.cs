using jobFinder.Application.Jobs.DTOs;
using jobFinder.Application.Jobs.Sources;

namespace jobFinder.Infrastructure.JobSources.Afriwork;

public sealed class AfriworkJobSource : IJobSource
{
    private readonly AfriworkClient _client;

    private const int PageSize = 100;

    public string PlatformName => "Afriwork";

    public AfriworkJobSource(AfriworkClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<ExternalJobDto>> GetJobsAsync(
        CancellationToken cancellationToken = default)
    {
        var jobs = new List<ExternalJobDto>();

        var offset = 0;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var afriworkJobs = await _client.GetJobsAsync(
                offset,
                PageSize,
                cancellationToken);

            if (afriworkJobs.Count == 0)
                break;

            foreach (var afriworkJob in afriworkJobs)
            {
                var job = AfriworkJobMapper.ToExternalJobDto(
                    afriworkJob);

                jobs.Add(job);
            }

            offset += afriworkJobs.Count;

            /*
             * If fewer jobs than the requested page size were returned,
             * we have reached the final page.
             */
            if (afriworkJobs.Count < PageSize)
                break;
        }

        return jobs;
    }
}