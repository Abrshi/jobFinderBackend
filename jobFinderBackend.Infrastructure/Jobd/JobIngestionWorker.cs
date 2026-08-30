
using jobFinder.Application.Jobs.Classification;
using jobFinder.Application.Jobs.Sources;
using jobFinder.Domain.Entities;
using jobFinderBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace jobFinderBackend.Infrastructure.Jobd;

public class JobIngestionWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<JobIngestionWorker> _logger;

    public JobIngestionWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<JobIngestionWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation("========================================");
        _logger.LogInformation("Job Ingestion Worker started.");
        _logger.LogInformation("========================================");

        await SynchronizeJobsAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(
                    TimeSpan.FromMinutes(1),
                    stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (stoppingToken.IsCancellationRequested)
                break;

            await SynchronizeJobsAsync(stoppingToken);
        }

        _logger.LogInformation(
            "Job Ingestion Worker stopped.");
    }

    private async Task SynchronizeJobsAsync(
        CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation(
                "========================================");

            _logger.LogInformation(
                "[{Time}] Starting job synchronization...",
                DateTime.Now);

            using var scope =
                _scopeFactory.CreateScope();

            var jobSource =
                scope.ServiceProvider
                    .GetRequiredService<IJobSource>();

            var db =
                scope.ServiceProvider
                    .GetRequiredService<JobFinderBackendDbContext>();

            var classifier =
                scope.ServiceProvider
                    .GetRequiredService<IJobClassifier>();

            var entityResolver =
                scope.ServiceProvider
                    .GetRequiredService<JobEntityResolver>();

            var connection =
                db.Database.GetDbConnection();

            _logger.LogInformation(
                "Database: {Database}",
                connection.Database);

            _logger.LogInformation(
                "Database server: {Server}",
                connection.DataSource);

            _logger.LogInformation(
                "Calling job source: {Platform}",
                jobSource.PlatformName);

            var externalJobs =
                await jobSource.GetJobsAsync(
                    stoppingToken);

            if (externalJobs == null)
            {
                _logger.LogWarning(
                    "Job source returned null.");

                return;
            }

            _logger.LogInformation(
                "Received {Count} jobs from {Platform}.",
                externalJobs.Count,
                jobSource.PlatformName);

            if (externalJobs.Count == 0)
            {
                _logger.LogInformation(
                    "No jobs received. Synchronization finished.");

                return;
            }

            var platform =
                await db.JobPlatforms
                    .FirstOrDefaultAsync(
                        x =>
                            x.Name ==
                            jobSource.PlatformName,
                        stoppingToken);

            if (platform == null)
            {
                platform = new JobPlatform
                {
                    Name = jobSource.PlatformName,
                    SourceType = "API",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                db.JobPlatforms.Add(platform);

                await db.SaveChangesAsync(
                    stoppingToken);

                _logger.LogInformation(
                    "Created job platform: {Platform} (Id: {Id})",
                    platform.Name,
                    platform.Id);
            }
            else
            {
                _logger.LogInformation(
                    "Using existing job platform: {Platform} (Id: {Id})",
                    platform.Name,
                    platform.Id);
            }

            var newJobs = 0;
            var updatedJobs = 0;
            var skippedJobs = 0;
            var classifiedJobs = 0;

            foreach (var externalJob in externalJobs)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                try
                {
                    if (string.IsNullOrWhiteSpace(
                            externalJob.ExternalId))
                    {
                        skippedJobs++;

                        _logger.LogWarning(
                            "Skipping job because ExternalId is empty.");

                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(externalJob.Title) &&
                        string.IsNullOrWhiteSpace(externalJob.Description))
                    {
                        skippedJobs++;

                        _logger.LogWarning(
                            "Skipping job {ExternalId} because both Title and Description are empty.",
                            externalJob.ExternalId);

                        continue;
                    }

                    var existingJob =
                        await db.Jobs
                            .Include(j => j.Category)
                            .Include(j => j.JobSkills)
                                .ThenInclude(js => js.Skill)
                            .FirstOrDefaultAsync(
                                x =>
                                    x.ExternalId ==
                                    externalJob.ExternalId
                                    &&
                                    x.JobPlatformId ==
                                    platform.Id,
                                stoppingToken);

                    if (existingJob == null)
                    {
                        _logger.LogInformation(
                            "Classifying new job: {Title}",
                            !string.IsNullOrWhiteSpace(externalJob.Title) ? externalJob.Title : externalJob.ExternalId);

                        var classification =
                            await classifier.ClassifyAsync(
                                externalJob,
                                stoppingToken);

                        classifiedJobs++;

                        var initialTitle = !string.IsNullOrWhiteSpace(externalJob.Title)
                            ? externalJob.Title
                            : (!string.IsNullOrWhiteSpace(classification.Title) ? classification.Title : "Untitled Position");

                        var job = new Job
                        {
                            ExternalId =
                                externalJob.ExternalId,

                            JobPlatformId =
                                platform.Id,

                            Title =
                                initialTitle,

                            Description =
                                externalJob.Description ?? string.Empty,

                            SalaryMin =
                                externalJob.SalaryMin,

                            SalaryMax =
                                externalJob.SalaryMax,

                            Country =
                                externalJob.Country,

                            City =
                                externalJob.City,

                            OriginalUrl =
                                externalJob.OriginalUrl,

                            PostedDate =
                                externalJob.PostedDate
                                ?? DateTime.UtcNow,

                            ExpiryDate =
                                externalJob.ExpiryDate,

                            Status =
                                "Active",

                            CreatedAt =
                                DateTime.UtcNow
                        };

                        await entityResolver.ApplyClassificationAsync(
                            job,
                            externalJob,
                            classification,
                            stoppingToken);

                        db.Jobs.Add(job);

                        newJobs++;

                        _logger.LogInformation(
                            "NEW JOB → {Title} | Category: {Category} | ExternalId: {ExternalId}",
                            job.Title,
                            classification.Category,
                            job.ExternalId);
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(externalJob.Title))
                        {
                            existingJob.Title = externalJob.Title;
                        }

                        existingJob.Description =
                            externalJob.Description
                            ?? string.Empty;

                        existingJob.SalaryMin =
                            externalJob.SalaryMin;

                        existingJob.SalaryMax =
                            externalJob.SalaryMax;

                        existingJob.OriginalUrl =
                            externalJob.OriginalUrl;

                        if (externalJob.PostedDate.HasValue)
                        {
                            existingJob.PostedDate =
                                externalJob.PostedDate.Value;
                        }

                        existingJob.ExpiryDate =
                            externalJob.ExpiryDate;

                        var needsReclassification =
                            string.IsNullOrWhiteSpace(existingJob.Title)
                            || existingJob.Title.Equals(
                                "Untitled Position",
                                StringComparison.OrdinalIgnoreCase)
                            || existingJob.Category.Name.Equals(
                                "Other",
                                StringComparison.OrdinalIgnoreCase)
                            || existingJob.JobSkills.Count == 0
                            || existingJob.CompanyId == null;

                        if (needsReclassification)
                        {
                            _logger.LogInformation(
                                "Re-classifying job: {Title}",
                                existingJob.Title);

                            var classification =
                                await classifier.ClassifyAsync(
                                    externalJob,
                                    stoppingToken);

                            classifiedJobs++;

                            await entityResolver.ApplyClassificationAsync(
                                existingJob,
                                externalJob,
                                classification,
                                stoppingToken);
                        }
                        else
                        {
                            existingJob.Country =
                                externalJob.Country
                                ?? existingJob.Country;

                            existingJob.City =
                                externalJob.City
                                ?? existingJob.City;

                            existingJob.EmploymentType =
                                externalJob.EmploymentType
                                ?? existingJob.EmploymentType;

                            existingJob.ExperienceLevel =
                                externalJob.ExperienceLevel
                                ?? existingJob.ExperienceLevel;

                            existingJob.RemoteType =
                                externalJob.RemoteType
                                ?? existingJob.RemoteType;

                            if (!string.IsNullOrWhiteSpace(
                                     externalJob.CompanyName)
                                 && existingJob.CompanyId == null)
                            {
                                var company =
                                    await entityResolver.GetOrCreateCompanyAsync(
                                        externalJob.CompanyName,
                                        externalJob.Country,
                                        null,
                                        stoppingToken);

                                existingJob.CompanyId = company?.Id;
                            }
                        }

                        updatedJobs++;

                        _logger.LogInformation(
                            "UPDATED JOB → {Title} | ExternalId: {ExternalId}",
                            existingJob.Title,
                            existingJob.ExternalId);
                    }
                }
                catch (Exception ex)
                {
                    skippedJobs++;

                    _logger.LogError(
                        ex,
                        "Failed to process job {ExternalId} ({Title}).",
                        externalJob.ExternalId,
                        externalJob.Title);
                }
            }

            _logger.LogInformation(
                "Saving {NewJobs} new jobs and {UpdatedJobs} updated jobs...",
                newJobs,
                updatedJobs);

            var savedChanges =
                await db.SaveChangesAsync(
                    stoppingToken);

            _logger.LogInformation(
                "SaveChangesAsync completed. EF saved {SavedChanges} database changes.",
                savedChanges);

            _logger.LogInformation(
                "========================================");

            _logger.LogInformation(
                "Job synchronization results:");

            _logger.LogInformation(
                "New jobs: {NewJobs}",
                newJobs);

            _logger.LogInformation(
                "Updated jobs: {UpdatedJobs}",
                updatedJobs);

            _logger.LogInformation(
                "Classified jobs: {ClassifiedJobs}",
                classifiedJobs);

            _logger.LogInformation(
                "Skipped jobs: {SkippedJobs}",
                skippedJobs);

            _logger.LogInformation(
                "Total received: {Total}",
                externalJobs.Count);

            _logger.LogInformation(
                "Database changes saved: {SavedChanges}",
                savedChanges);

            _logger.LogInformation(
                "========================================");

            _logger.LogInformation(
                "[{Time}] Job synchronization finished.",
                DateTime.Now);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation(
                "Job synchronization was cancelled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while synchronizing jobs.");
        }
    }
}
