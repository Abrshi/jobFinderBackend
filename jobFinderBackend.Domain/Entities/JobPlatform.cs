namespace jobFinder.Domain.Entities;

public class JobPlatform
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Website { get; set; }

    public string? Logo { get; set; }


    // How jobs are collected
    // API, Scraper, RSS, Manual
    public string SourceType { get; set; } = null!;


    public bool IsActive { get; set; } = true;


    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    // Navigation

    public ICollection<UserJobSource> UserJobSources { get; set; }
        = new List<UserJobSource>();

    public ICollection<Job> Jobs { get; set; }
        = new List<Job>();
}