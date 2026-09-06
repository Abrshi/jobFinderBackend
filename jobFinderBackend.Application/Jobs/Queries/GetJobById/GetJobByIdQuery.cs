using jobFinderBackend.Application.Interfaces;
using jobFinderBackend.Application.Jobs.DTOs;
using MediatR;

namespace jobFinderBackend.Application.Jobs.Queries.GetJobById;

public record GetJobByIdQuery(int Id) : IRequest<JobListItemResponse?>;

public class GetJobByIdQueryHandler
    : IRequestHandler<GetJobByIdQuery, JobListItemResponse?>
{
    private readonly IJobRepository _jobRepository;

    public GetJobByIdQueryHandler(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<JobListItemResponse?> Handle(
        GetJobByIdQuery request,
        CancellationToken cancellationToken)
    {
        var j = await _jobRepository.GetByIdAsync(request.Id, cancellationToken);

        if (j == null)
        {
            return null;
        }

        return new JobListItemResponse
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
        };
    }
}
