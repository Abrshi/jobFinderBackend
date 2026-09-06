namespace jobFinder.Domain.Entities;

public class GeneratedCoverLetter
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? JobId { get; set; }

    // The actual generated cover letter text
    public string Content { get; set; } = null!;

    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Users User { get; set; } = null!;

    public Job? Job { get; set; }

    public ICollection<Application> Applications { get; set; }
        = new List<Application>();
}