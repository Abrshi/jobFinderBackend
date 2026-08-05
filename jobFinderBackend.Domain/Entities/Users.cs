namespace jobFinder.Domain.Entities;

public class Users
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? ProfileImage { get; set; }

    public int SubscriptionPlanId { get; set; }

    public string Status { get; set; } = "Active";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }


    // Navigation

    public SubscriptionPlans SubscriptionPlan { get; set; } = null!;

    public UserProfile? UserProfile { get; set; }

    public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();

    public ICollection<Experience> Experiences { get; set; } = new List<Experience>();

    public ICollection<Education> Educations { get; set; } = new List<Education>();

    public ICollection<UserJobSource> UserJobSources { get; set; } = new List<UserJobSource>();
}