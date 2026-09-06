namespace jobFinderBackend.Application.Applications.DTOs;

public class CvContent
{
    public CvPersonalInfo PersonalInfo { get; set; } = new();

    public string Summary { get; set; } = string.Empty;

    public List<string> Skills { get; set; } = [];

    public List<CvExperience> Experience { get; set; } = [];

    public List<CvEducation> Education { get; set; } = [];
}

public class CvPersonalInfo
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }
}

public class CvExperience
{
    public string? Company { get; set; }

    public string Position { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string StartDate { get; set; } = string.Empty;

    public string? EndDate { get; set; }

    public bool IsCurrent { get; set; }
}

public class CvEducation
{
    public string Institution { get; set; } = string.Empty;

    public string Degree { get; set; } = string.Empty;

    public string? FieldOfStudy { get; set; }

    public string? StartDate { get; set; }

    public string? EndDate { get; set; }
}