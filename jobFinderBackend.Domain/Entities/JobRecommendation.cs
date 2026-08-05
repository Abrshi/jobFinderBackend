namespace jobFinder.Domain.Entities;

public class JobRecommendation
{
    public int Id { get; set; }


    public int UserId { get; set; }


    public int JobId { get; set; }



    public decimal MatchScore { get; set; }


    public string? Reason { get; set; }


    public DateTime RecommendedAt { get; set; }
        = DateTime.UtcNow;



    // Navigation


    public Users User { get; set; } = null!;


    public Job Job { get; set; } = null!;
}