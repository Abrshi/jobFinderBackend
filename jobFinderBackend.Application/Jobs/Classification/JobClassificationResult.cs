namespace jobFinder.Application.Jobs.Classification;

public sealed class JobClassificationResult
{
    public string? Title { get; set; }

    public string Category { get; set; } = "Other";

    public string? CompanyName { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public string? EmploymentType { get; set; }

    public string? ExperienceLevel { get; set; }

    public string? RemoteType { get; set; }

    public string? EducationLevel { get; set; }

    public string Gender { get; set; } = "ANY";

    public int? MinimumExperienceYears { get; set; }

    public string? CompanyIndustry { get; set; }

    public List<string> Skills { get; set; } = new();
}
