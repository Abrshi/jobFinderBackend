
using jobFinder.Domain.Entities;
using jobFinderBackend.Application.Interfaces;
using jobFinderBackend.Application.Jobs.DTOs;
using jobFinderBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace jobFinderBackend.Infrastructure.Persistence.Repositories;

public class JobRepository : IJobRepository
{
    private readonly JobFinderBackendDbContext _context;

    public JobRepository(JobFinderBackendDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Job> Jobs, int TotalCount)> GetFilteredAsync(
        JobFilterParams filters,
        List<int> skillIds,
        List<int> platformIds,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Jobs
            .AsNoTracking()
            .Include(j => j.JobPlatform)
            .Include(j => j.Category)
            .Include(j => j.Company)
            .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
            .AsQueryable();

        // ---------------------------------------------------------
        // USER PERSONALIZATION
        // ---------------------------------------------------------

        // Only return jobs from platforms selected by the user.
        //
        // If the user has no selected platforms, return no jobs.
        if (platformIds.Count == 0)
        {
            query = query.Where(j => false);
        }
        else
        {
            query = query.Where(j =>
                platformIds.Contains(j.JobPlatformId));
        }

        // Only return jobs that contain at least one skill
        // selected by the user.
        //
        // If the user has no skills, don't apply skill filtering.
        if (skillIds.Count > 0)
        {
            query = query.Where(j =>
                j.JobSkills.Any(js =>
                    skillIds.Contains(js.SkillId)));
        }

        // ---------------------------------------------------------
        // NORMAL JOB FILTERS
        // ---------------------------------------------------------

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var term = filters.Search.Trim().ToLower();

            query = query.Where(j =>
                j.Title.ToLower().Contains(term) ||
                j.Description.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(filters.EmploymentType))
        {
            query = query.Where(j =>
                j.EmploymentType != null &&
                j.EmploymentType.ToLower() ==
                filters.EmploymentType.Trim().ToLower());
        }

        if (!string.IsNullOrWhiteSpace(filters.ExperienceLevel))
        {
            query = query.Where(j =>
                j.ExperienceLevel != null &&
                j.ExperienceLevel.ToLower() ==
                filters.ExperienceLevel.Trim().ToLower());
        }

        if (!string.IsNullOrWhiteSpace(filters.RemoteType))
        {
            query = query.Where(j =>
                j.RemoteType != null &&
                j.RemoteType.ToLower() ==
                filters.RemoteType.Trim().ToLower());
        }

        if (!string.IsNullOrWhiteSpace(filters.Status))
        {
            query = query.Where(j =>
                j.Status.ToLower() ==
                filters.Status.Trim().ToLower());
        }
        else
        {
            query = query.Where(j =>
                j.Status == "Active");
        }

        if (!string.IsNullOrWhiteSpace(filters.Country))
        {
            query = query.Where(j =>
                j.Country != null &&
                j.Country.ToLower() ==
                filters.Country.Trim().ToLower());
        }

        if (!string.IsNullOrWhiteSpace(filters.City))
        {
            query = query.Where(j =>
                j.City != null &&
                j.City.ToLower().Contains(
                    filters.City.Trim().ToLower()));
        }

        if (filters.CategoryId.HasValue)
        {
            query = query.Where(j =>
                j.CategoryId == filters.CategoryId.Value);
        }

        // This is the user's manual platform filter.
        // It is combined with the user's selected platforms above.
        if (filters.PlatformId.HasValue)
        {
            query = query.Where(j =>
                j.JobPlatformId == filters.PlatformId.Value);
        }

        if (filters.CompanyId.HasValue)
        {
            query = query.Where(j =>
                j.CompanyId == filters.CompanyId.Value);
        }

        if (filters.SalaryMin.HasValue)
        {
            query = query.Where(j =>
                j.SalaryMax == null ||
                j.SalaryMax >= filters.SalaryMin.Value);
        }

        if (filters.SalaryMax.HasValue)
        {
            query = query.Where(j =>
                j.SalaryMin == null ||
                j.SalaryMin <= filters.SalaryMax.Value);
        }

        if (filters.PostedAfter.HasValue)
        {
            query = query.Where(j =>
                j.PostedDate >= filters.PostedAfter.Value);
        }

        if (filters.PostedBefore.HasValue)
        {
            query = query.Where(j =>
                j.PostedDate <= filters.PostedBefore.Value);
        }

        // ---------------------------------------------------------
        // TOTAL COUNT
        // ---------------------------------------------------------

        var totalCount = await query.CountAsync(
            cancellationToken);

        // ---------------------------------------------------------
        // SORTING
        // ---------------------------------------------------------

        query = ApplySorting(query, filters);

        // ---------------------------------------------------------
        // PAGINATION
        // ---------------------------------------------------------

        var page = filters.Page < 1
            ? 1
            : filters.Page;

        var pageSize = filters.PageSize switch
        {
            < 1 => 20,
            > 100 => 100,
            _ => filters.PageSize
        };

        var jobs = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (jobs, totalCount);
    }

    public async Task<Job?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Jobs
            .AsNoTracking()
            .Include(j => j.JobPlatform)
            .Include(j => j.Category)
            .Include(j => j.Company)
            .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
            .FirstOrDefaultAsync(
                j => j.Id == id,
                cancellationToken);
    }

    private static IQueryable<Job> ApplySorting(
        IQueryable<Job> query,
        JobFilterParams filters)
    {
        var descending =
            filters.SortDirection.Equals(
                "desc",
                StringComparison.OrdinalIgnoreCase);

        return filters.SortBy.ToLower() switch
        {
            "title" => descending
                ? query.OrderByDescending(j => j.Title)
                : query.OrderBy(j => j.Title),

            "salary" => descending
                ? query.OrderByDescending(j => j.SalaryMax)
                    .ThenByDescending(j => j.SalaryMin)
                : query.OrderBy(j => j.SalaryMin)
                    .ThenBy(j => j.SalaryMax),

            "createdat" => descending
                ? query.OrderByDescending(j => j.CreatedAt)
                : query.OrderBy(j => j.CreatedAt),

            _ => descending
                ? query.OrderByDescending(j => j.PostedDate)
                : query.OrderBy(j => j.PostedDate)
        };
    }
}
