namespace jobFinder.Domain.Entities;

public class UserProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Headline { get; set; }

    public string? Summary { get; set; }

    public string? CurrentJob { get; set; }

    public int? YearsOfExperience { get; set; }

    public string? PreferredCountry { get; set; }

    public string? PreferredCity { get; set; }

    public decimal? ExpectedSalary { get; set; }


    public Users User { get; set; } = null!;
}