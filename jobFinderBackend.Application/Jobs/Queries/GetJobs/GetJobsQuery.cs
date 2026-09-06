using jobFinderBackend.Application.Interfaces;
using jobFinderBackend.Application.Jobs.DTOs;
using MediatR;

namespace jobFinderBackend.Application.Jobs.Queries.GetJobs;

public record GetJobsQuery(JobFilterParams Filters)
    : IRequest<GetJobsResponse>;

public class GetJobsQueryHandler
    : IRequestHandler<GetJobsQuery, GetJobsResponse>
{
    private readonly IJobRepository _jobRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUserSkillRepository _userSkillRepository;
    private readonly IUserJobPlatformRepository _userJobPlatformRepository;

    public GetJobsQueryHandler(
        IJobRepository jobRepository,
        ICurrentUserService currentUser,
        IUserSkillRepository userSkillRepository,
        IUserJobPlatformRepository userJobPlatformRepository)
    {
        _jobRepository = jobRepository;
        _currentUser = currentUser;
        _userSkillRepository = userSkillRepository;
        _userJobPlatformRepository = userJobPlatformRepository;
    }

    public async Task<GetJobsResponse> Handle(
        GetJobsQuery request,
        CancellationToken cancellationToken)
    {
        // Get the currently authenticated user
        if (_currentUser.UserId is null)
        {
            throw new UnauthorizedAccessException();
        }

        var userId = _currentUser.UserId.Value;

        // Normalize pagination
        var filters = request.Filters;

        var page = filters.Page < 1
            ? 1
            : filters.Page;

        var pageSize = filters.PageSize switch
        {
            < 1 => 20,
            > 100 => 100,
            _ => filters.PageSize
        };

        filters.Page = page;
        filters.PageSize = pageSize;

        // Get user's selected skills
        var skillIds = await _userSkillRepository
            .GetUserSkillIdsAsync(userId);

        // Get user's selected job platforms
        var userJobPlatforms = await _userJobPlatformRepository
            .GetForUserAsync(userId);

        var platformIds = userJobPlatforms
            .Select(x => x.JobPlatformId)
            .ToList();

        // Get jobs based on:
        // 1. User's normal filters
        // 2. User's skills
        // 3. User's selected job platforms
        var (jobs, totalCount) =
            await _jobRepository.GetFilteredAsync(
                filters,
                skillIds,
                platformIds,
                cancellationToken);

        return new GetJobsResponse
        {
            Jobs = jobs.Select(j => new JobListItemResponse
            {
                Id = j.Id,
                ExternalId = j.ExternalId,
                Title = j.Title,
                Description = j.Description,

                EmploymentType = j.EmploymentType,
                ExperienceLevel = j.ExperienceLevel,

                SalaryMin = j.SalaryMin,
                SalaryMax = j.SalaryMax,

                Country = j.Country,
                City = j.City,

                RemoteType = j.RemoteType,

                OriginalUrl = j.OriginalUrl,

                PostedDate = j.PostedDate,
                ExpiryDate = j.ExpiryDate,

                Status = j.Status,
                CreatedAt = j.CreatedAt,

                Platform = new JobPlatformSummary
                {
                    Id = j.JobPlatform.Id,
                    Name = j.JobPlatform.Name,
                    Website = j.JobPlatform.Website,
                    Logo = j.JobPlatform.Logo
                },

                Category = new JobCategorySummary
                {
                    Id = j.Category.Id,
                    Name = j.Category.Name
                },

                Company = j.Company == null
                    ? null
                    : new CompanySummary
                    {
                        Id = j.Company.Id,
                        Name = j.Company.Name,
                        Website = j.Company.Website,
                        Logo = j.Company.Logo,
                        Industry = j.Company.Industry,
                        Country = j.Company.Country
                    },

                Skills = j.JobSkills
                    .Select(js => new JobSkillSummary
                    {
                        Id = js.Skill.Id,
                        Name = js.Skill.Name,
                        Category = js.Skill.Category,
                        Importance = js.Importance
                    })
                    .ToList()

            }).ToList(),

            TotalCount = totalCount,

            Page = page,

            PageSize = pageSize,

            TotalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize)
        };
    }
}

