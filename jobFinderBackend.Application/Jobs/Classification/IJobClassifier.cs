using jobFinder.Application.Jobs.DTOs;

namespace jobFinder.Application.Jobs.Classification;

public interface IJobClassifier
{
    Task<JobClassificationResult> ClassifyAsync(
        ExternalJobDto job,
        CancellationToken cancellationToken = default);
}