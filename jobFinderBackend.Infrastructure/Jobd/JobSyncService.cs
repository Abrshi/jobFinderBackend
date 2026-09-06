using jobFinder.Application.Jobs.Classification;
using jobFinder.Application.Jobs.Sources;
using jobFinder.Domain.Entities;
using jobFinderBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace jobFinderBackend.Infrastructure.Jobd;

public class JobSyncService : IJobSyncService
{
    private readonly IJobSource _jobSource;
    private readonly JobFinderBackendDbContext _db;
    private readonly IJobClassifier _classifier;
    private readonly JobEntityResolver _entityResolver;
    private readonly ILogger<JobSyncService> _logger;

    public JobSyncService(
        IJobSource jobSource,
        JobFinderBackendDbContext db,
        IJobClassifier classifier,
        JobEntityResolver entityResolver,
        ILogger<JobSyncService> logger)
    {
        _jobSource = jobSource;
        _db = db;
        _classifier = classifier;
        _entityResolver = entityResolver;
        _logger = logger;
    }

    public async Task<JobSyncResultDto> SynchronizeJobsAsync(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("========================================");
        _logger.LogInformation("[{Time}] Starting job synchronization for {Platform}...", DateTime.Now, _jobSource.PlatformName);

        var externalJobs = await _jobSource.GetJobsAsync(cancellationToken);

        if (externalJobs == null || externalJobs.Count == 0)
        {
            _logger.LogInformation("No jobs received from source.");
            return new JobSyncResultDto(0, 0, 0, 0, 0);
        }

        _logger.LogInformation("Received {Count} jobs from {Platform}.", externalJobs.Count, _jobSource.PlatformName);

        var platform = await _db.JobPlatforms
            .FirstOrDefaultAsync(x => x.Name == _jobSource.PlatformName, cancellationToken);

        if (platform == null)
        {
            platform = new JobPlatform
            {
                Name = _jobSource.PlatformName,
                SourceType = "API",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.JobPlatforms.Add(platform);
            await _db.SaveChangesAsync(cancellationToken);
        }

        var newJobs = 0;
        var updatedJobs = 0;
        var skippedJobs = 0;
        var classifiedJobs = 0;

        foreach (var externalJob in externalJobs)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                if (string.IsNullOrWhiteSpace(externalJob.ExternalId) ||
                    (string.IsNullOrWhiteSpace(externalJob.Title) && string.IsNullOrWhiteSpace(externalJob.Description)))
                {
                    skippedJobs++;
                    continue;
                }

                var existingJob = await _db.Jobs
                    .Include(j => j.Category)
                    .Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                    .FirstOrDefaultAsync(x => x.ExternalId == externalJob.ExternalId && x.JobPlatformId == platform.Id, cancellationToken);

                if (existingJob == null)
                {
                    _logger.LogInformation("Classifying new job: {Title}", !string.IsNullOrWhiteSpace(externalJob.Title) ? externalJob.Title : externalJob.ExternalId);

                    var classification = await _classifier.ClassifyAsync(externalJob, cancellationToken);
                    classifiedJobs++;

                    var initialTitle = !string.IsNullOrWhiteSpace(externalJob.Title)
                        ? externalJob.Title
                        : (!string.IsNullOrWhiteSpace(classification.Title) ? classification.Title : "Untitled Position");

                    var job = new Job
                    {
                        ExternalId = externalJob.ExternalId,
                        JobPlatformId = platform.Id,
                        Title = initialTitle,
                        Description = externalJob.Description ?? string.Empty,
                        SalaryMin = externalJob.SalaryMin,
                        SalaryMax = externalJob.SalaryMax,
                        Country = externalJob.Country,
                        City = externalJob.City,
                        OriginalUrl = externalJob.OriginalUrl,
                        PostedDate = externalJob.PostedDate ?? DateTime.UtcNow,
                        ExpiryDate = externalJob.ExpiryDate,
                        Status = "Active",
                        CreatedAt = DateTime.UtcNow
                    };

                    await _entityResolver.ApplyClassificationAsync(job, externalJob, classification, cancellationToken);
                    _db.Jobs.Add(job);
                    newJobs++;

                    _logger.LogInformation("NEW JOB → {Title} | Category: {Category} | ExternalId: {ExternalId}", job.Title, classification.Category, job.ExternalId);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(externalJob.Title)) existingJob.Title = externalJob.Title;
                    existingJob.Description = externalJob.Description ?? string.Empty;
                    existingJob.SalaryMin = externalJob.SalaryMin;
                    existingJob.SalaryMax = externalJob.SalaryMax;
                    existingJob.OriginalUrl = externalJob.OriginalUrl;
                    if (externalJob.PostedDate.HasValue) existingJob.PostedDate = externalJob.PostedDate.Value;
                    existingJob.ExpiryDate = externalJob.ExpiryDate;

                    var needsReclassification = string.IsNullOrWhiteSpace(existingJob.Title)
                        || existingJob.Title.Equals("Untitled Position", StringComparison.OrdinalIgnoreCase)
                        || existingJob.Category == null
                        || existingJob.Category.Name.Equals("Other", StringComparison.OrdinalIgnoreCase)
                        || existingJob.JobSkills.Count == 0
                        || existingJob.CompanyId == null;

                    if (needsReclassification)
                    {
                        _logger.LogInformation("Re-classifying job: {Title}", existingJob.Title);
                        var classification = await _classifier.ClassifyAsync(externalJob, cancellationToken);
                        classifiedJobs++;
                        await _entityResolver.ApplyClassificationAsync(existingJob, externalJob, classification, cancellationToken);
                    }
                    else
                    {
                        existingJob.Country = externalJob.Country ?? existingJob.Country;
                        existingJob.City = externalJob.City ?? existingJob.City;
                        existingJob.EmploymentType = externalJob.EmploymentType ?? existingJob.EmploymentType;
                        existingJob.ExperienceLevel = externalJob.ExperienceLevel ?? existingJob.ExperienceLevel;
                        existingJob.RemoteType = externalJob.RemoteType ?? existingJob.RemoteType;

                        if (!string.IsNullOrWhiteSpace(externalJob.CompanyName) && existingJob.CompanyId == null)
                        {
                            var company = await _entityResolver.GetOrCreateCompanyAsync(
                                externalJob.CompanyName, externalJob.Country, null, cancellationToken);
                            existingJob.CompanyId = company?.Id;
                        }
                    }

                    updatedJobs++;
                }
            }
            catch (Exception ex)
            {
                skippedJobs++;
                _logger.LogError(ex, "Failed to process job {ExternalId} ({Title}).", externalJob.ExternalId, externalJob.Title);
            }
        }

        var savedChanges = await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Job synchronization finished. New: {NewJobs}, Updated: {UpdatedJobs}, Classified: {ClassifiedJobs}, Saved: {SavedChanges}",
            newJobs, updatedJobs, classifiedJobs, savedChanges);

        return new JobSyncResultDto(newJobs, updatedJobs, classifiedJobs, skippedJobs, externalJobs.Count);
    }
}
