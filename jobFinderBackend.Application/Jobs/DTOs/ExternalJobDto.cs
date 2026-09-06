namespace jobFinder.Application.Jobs.DTOs;

public class ExternalJobDto
{
    public string ExternalId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? EmploymentType { get; set; }

    public string? ExperienceLevel { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public string? RemoteType { get; set; }

    public DateTime? PostedDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string? CompanyName { get; set; }

    public List<string> Skills { get; set; } = new();

    public decimal? SalaryMin { get; set; }

    public decimal? SalaryMax { get; set; }

    public string? SalaryCurrency { get; set; }

    public string? OriginalUrl { get; set; }
}