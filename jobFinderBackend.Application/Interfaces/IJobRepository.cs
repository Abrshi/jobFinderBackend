using jobFinder.Domain.Entities;
using jobFinderBackend.Application.Jobs.DTOs;

namespace jobFinderBackend.Application.Interfaces;

public interface IJobRepository
{
    Task<(List<Job> Jobs, int TotalCount)> GetFilteredAsync(
       JobFilterParams filters,
        List<int> skillIds,
        List<int> platformIds,
        CancellationToken cancellationToken);

    Task<Job?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}
