namespace jobFinderBackend.Application.Jobs.DTOs;

public class GetJobsResponse
{
    public List<JobListItemResponse> Jobs { get; set; } = new();

    public int TotalCount { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalPages { get; set; }
}

public class JobListItemResponse
{
    public int Id { get; set; }

    public string ExternalId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? EmploymentType { get; set; }

    public string? ExperienceLevel { get; set; }

    public decimal? SalaryMin { get; set; }

    public decimal? SalaryMax { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public string? RemoteType { get; set; }

    public string? OriginalUrl { get; set; }

    public DateTime PostedDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public JobPlatformSummary Platform { get; set; } = null!;

    public JobCategorySummary Category { get; set; } = null!;

    public CompanySummary? Company { get; set; }

    public List<JobSkillSummary> Skills { get; set; } = new();
}

public class JobPlatformSummary
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Website { get; set; }

    public string? Logo { get; set; }
}

public class JobCategorySummary
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
}

public class CompanySummary
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Website { get; set; }

    public string? Logo { get; set; }

    public string? Industry { get; set; }

    public string? Country { get; set; }
}

public class JobSkillSummary
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Category { get; set; }

    public string Importance { get; set; } = null!;
}
