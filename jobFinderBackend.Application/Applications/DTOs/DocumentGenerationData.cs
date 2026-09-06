namespace jobFinderBackend.Application.Applications.DTOs;

public class DocumentGenerationData
{
    public CandidateData Candidate { get; set; } = null!;

    public JobData Job { get; set; } = null!;
}

public class CandidateData
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public CandidateProfileData? Profile { get; set; }

    public List<CandidateEducationData> Education { get; set; } = [];

    public List<CandidateExperienceData> Experience { get; set; } = [];

    public List<CandidateSkillData> Skills { get; set; } = [];
}

public class CandidateProfileData
{
    public string? Headline { get; set; }

    public string? Summary { get; set; }

    public string? CurrentJob { get; set; }

    public int? YearsOfExperience { get; set; }
}

public class CandidateEducationData
{
    public string Institution { get; set; } = string.Empty;

    public string Degree { get; set; } = string.Empty;

    public string? FieldOfStudy { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}

public class CandidateExperienceData
{
    public string? Company { get; set; }

    public string Position { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsCurrent { get; set; }
}

public class CandidateSkillData
{
    public string Name { get; set; } = string.Empty;

    public string? Level { get; set; }

    public int? YearsOfExperience { get; set; }
}

public class JobData
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? EmploymentType { get; set; }

    public string? ExperienceLevel { get; set; }

    public decimal? SalaryMin { get; set; }

    public decimal? SalaryMax { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public string? RemoteType { get; set; }

    public string? Company { get; set; }

    public string? CompanyIndustry { get; set; }

    public string? Category { get; set; }

    public List<JobSkillData> Skills { get; set; } = [];
}

public class JobSkillData
{
    public string Name { get; set; } = string.Empty;

    public string Importance { get; set; } = string.Empty;
}