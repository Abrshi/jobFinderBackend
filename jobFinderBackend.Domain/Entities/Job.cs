namespace jobFinder.Domain.Entities;

public class Job
{
    public int Id { get; set; }


    public int JobPlatformId { get; set; }


    public int? CompanyId { get; set; }


    public int CategoryId { get; set; }



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



    // Active
    // Expired
    // Removed

    public string Status { get; set; }
        = "Active";



    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;



    // Navigation


    public JobPlatform JobPlatform { get; set; } = null!;


    public Company? Company { get; set; }


    public JobCategory Category { get; set; } = null!;


    public ICollection<JobSkill> JobSkills { get; set; }
        = new List<JobSkill>();


    public ICollection<JobRecommendation> Recommendations { get; set; }
        = new List<JobRecommendation>();


    public ICollection<Application> Applications { get; set; }
        = new List<Application>();
}