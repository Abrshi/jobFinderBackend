
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
            using var scope = _scopeFactory.CreateScope();
            var syncService = scope.ServiceProvider.GetRequiredService<IJobSyncService>();
            await syncService.SynchronizeJobsAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Job synchronization was cancelled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while synchronizing jobs.");
        }
    }
}
