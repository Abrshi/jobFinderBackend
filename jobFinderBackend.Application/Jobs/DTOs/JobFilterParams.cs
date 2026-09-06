namespace jobFinderBackend.Application.Jobs.DTOs;

public class JobFilterParams
{
    public string? Search { get; set; }

    public string? EmploymentType { get; set; }

    public string? ExperienceLevel { get; set; }

    public string? RemoteType { get; set; }

    public string? Status { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public int? CategoryId { get; set; }

    public int? PlatformId { get; set; }

    public int? CompanyId { get; set; }

    public decimal? SalaryMin { get; set; }

    public decimal? SalaryMax { get; set; }

    public DateTime? PostedAfter { get; set; }

    public DateTime? PostedBefore { get; set; }

    public string SortBy { get; set; } = "postedDate";

    public string SortDirection { get; set; } = "desc";

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
